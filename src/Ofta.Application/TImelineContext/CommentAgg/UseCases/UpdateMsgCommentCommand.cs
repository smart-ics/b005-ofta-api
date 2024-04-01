using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Ofta.Application.TImelineContext.CommentAgg.Workers;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record UpdateMsgCommentCommand(string CommentId, string Msg, string UserOftaId)
    : IRequest, ICommentKey, IUserOftaKey;

public class UpdateMsgCommentHandler : IRequestHandler<UpdateMsgCommentCommand>
{
    private readonly ICommentBuilder _builder;
    private readonly ICommentWriter _writer;
    private readonly IValidator<UpdateMsgCommentCommand> _guard;
    private readonly IMemoryCache _cache;

    public UpdateMsgCommentHandler(ICommentBuilder builder, 
        ICommentWriter writer, 
        IValidator<UpdateMsgCommentCommand> guard, 
        IMemoryCache cache)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _cache = cache;
    }

    public Task<Unit> Handle(UpdateMsgCommentCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = 
            _cache.Get<CommentModel>($"Comment-{request.CommentId}") 
            ?? _builder
                .Load(new CommentModel(request.CommentId))
                .Build();

        agg = _builder
            .Attach(agg)
            .Msg(request.Msg)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class UpdateMsgCommnetGuard : AbstractValidator<UpdateMsgCommentCommand>
{
    private readonly ICommentBuilder _builder;
    private readonly IMemoryCache _cache;
    
    public UpdateMsgCommnetGuard(ICommentBuilder builder, 
        IMemoryCache cache)
    {
        _builder = builder;
        _cache = cache;

        RuleFor(x => x.CommentId).NotEmpty();
        RuleFor(x => x.Msg).NotEmpty();
        RuleFor(x => x.UserOftaId)
            .NotEmpty()
            .Must(CompareValue)
            .WithMessage("User has no access to update comment");
    }
    
    private bool CompareValue(UpdateMsgCommentCommand model, string value)
    {
        //  try get CommentAgg from cache
        var comment = _cache.Get<CommentModel>($"Comment-{model.CommentId}");
        if (comment is not null) 
            return value == comment.UserOftaId;
        
        //  cache not found; retrieve from db
        ICommentKey commentKey = new CommentModel(model.CommentId);
        comment = _builder.Load(commentKey).Build();

        //  cache CommentAgg
        var cacheOption = new MemoryCacheEntryOptions {SlidingExpiration = TimeSpan.FromSeconds(1)};
        _cache.Set($"Comment-{model.CommentId}", comment, cacheOption);
        
        //  return
        return value == comment.UserOftaId;
    }    
    
}