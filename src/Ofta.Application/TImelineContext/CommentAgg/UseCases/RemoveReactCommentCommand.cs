using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.CommentAgg.Workers;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record RemoveReactCommentCommand(string CommentId, string UserOftaId)
    : IRequest, ICommentKey, IUserOftaKey;

public class RemvoeReactCommentHandler : IRequestHandler<RemoveReactCommentCommand>
{
    private readonly ICommentBuilder _builder;
    private readonly ICommentWriter _writer;
    private readonly IValidator<RemoveReactCommentCommand> _guard;

    public RemvoeReactCommentHandler(ICommentBuilder builder, 
        ICommentWriter writer, 
        IValidator<RemoveReactCommentCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(RemoveReactCommentCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .RemoveReact(new UserOftaModel(request.UserOftaId))
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class RemoveReactCommentGuard : AbstractValidator<RemoveReactCommentCommand>
{
    public RemoveReactCommentGuard()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.CommentId).NotEmpty();
    }
}