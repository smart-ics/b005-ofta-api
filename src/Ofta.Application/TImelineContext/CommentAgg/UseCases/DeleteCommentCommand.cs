using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Ofta.Application.TImelineContext.CommentAgg.Workers;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record DeleteCommentCommand(string CommentId, string UserOftaId)
    : IRequest, ICommentKey, IUserOftaKey;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly ICommentBuilder _builder;
    private readonly ICommentWriter _writer;
    private readonly IValidator<DeleteCommentCommand> _guard;
    private readonly IMemoryCache _cache;
    private readonly IMediator _mediator;

    public DeleteCommentHandler(ICommentBuilder builder, 
        ICommentWriter writer, 
        IValidator<DeleteCommentCommand> guard, 
        IMemoryCache cache, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _cache = cache;
        _mediator = mediator;
    }

    public Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _cache.Get<CommentModel>($"Comment-{request.CommentId}")
                  ?? _builder.Load(request).Build();
        
        //  WRITE
        _writer.Delete(agg);
        _mediator.Publish(new DeletedCommentEvent(agg, request), cancellationToken);
        return Task.FromResult(Unit.Value);
    }
}

public record DeletedCommentEvent(
    CommentModel Aggregate,
    DeleteCommentCommand Command
    ) : INotification;

public class DeleteCommentGuard : AbstractValidator<DeleteCommentCommand>
{
    private readonly ICommentBuilder _builder;
    private readonly IMemoryCache _cache;
    public DeleteCommentGuard(ICommentBuilder builder, IMemoryCache cache)
    {
        _builder = builder;
        _cache = cache;
        
        RuleFor(x => x.CommentId)
            .NotEmpty()
            .Must(Exist)
            .WithMessage($"Comment not found");

        RuleFor(x => x.UserOftaId)
            .NotEmpty()
            .Must(TheCreator)
            .WithMessage("User has no access right");
    }

    private bool Exist(DeleteCommentCommand cmd, string value)
    {
        var comment = GetCommentNullIfNotFound(cmd);
        return comment is not null;
    }
    private bool TheCreator(DeleteCommentCommand cmd, string value)
    {
        var comment = GetCommentNullIfNotFound(cmd);
        if (comment is null)
            return false;

        return comment.UserOftaId == value;
    }

    private CommentModel? GetCommentNullIfNotFound(ICommentKey commentKey)
    {
        //  from cache
        var comment = _cache.Get<CommentModel>($"Comment-{commentKey.CommentId}");

        //  from db
        if (comment is null)
        {
            var fallback = Policy<CommentModel?>
                .Handle<KeyNotFoundException>()
                .Fallback(() => null);
            comment = fallback.Execute(() =>
            {
                var komen = _builder.Load(commentKey).Build();
        
                //  set cache
                var cacheOption = new MemoryCacheEntryOptions {SlidingExpiration = TimeSpan.FromSeconds(1)};
                _cache.Set($"Comment-{komen.CommentId}", komen, cacheOption);
                return komen;
            });
        }

        return comment;
    }
    
}