using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.CommentAgg.Workers;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record AddReactCommentCommand(string CommentId, string UserOftaId) 
    : IRequest, ICommentKey, IUserOftaKey;

public class AddReactCommentHandler : IRequestHandler<AddReactCommentCommand>
{
    private readonly ICommentBuilder _builder;
    private readonly ICommentWriter _writer;
    private readonly IValidator<AddReactCommentCommand> _guard;

    public AddReactCommentHandler(ICommentBuilder builder,
        ICommentWriter writer,
        IValidator<AddReactCommentCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(AddReactCommentCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .AddReact(request)
            .Build();
        
        //  WRITER
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class AddReactCommentGuard : AbstractValidator<AddReactCommentCommand>
{
    public AddReactCommentGuard()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.CommentId).NotEmpty();
    }
}