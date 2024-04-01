using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.CommentAgg.Workers;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record CreateCommentCommand(string Msg, string PostId, string UserOftaId)
    : IRequest<CreateCommentResponse>, IPostKey, IUserOftaKey;

public record CreateCommentResponse(string CommentId);

public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, CreateCommentResponse>
{
    private readonly ICommentBuilder _builder;
    private readonly ICommentWriter _writer;
    private readonly IValidator<CreateCommentCommand> _guard;
    private readonly IMediator _mediator;

    public CreateCommentHandler(ICommentBuilder builder, 
        ICommentWriter writer, 
        IValidator<CreateCommentCommand> guard, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
    }

    public Task<CreateCommentResponse> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Create()
            .Post(request)
            .Msg(request.Msg)
            .User(request)
            .Build();
        
        //  WRITE
        agg = _writer.Save(agg);
        _mediator.Publish(new CreatedCommentEvent(agg, request), cancellationToken);
        return Task.FromResult(new CreateCommentResponse(agg.CommentId));
    }
}

public record CreatedCommentEvent(
    CommentModel Aggregate,
    CreateCommentCommand Command) : INotification;

public class CreateCommentGuard : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentGuard()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.Msg).NotEmpty();
        RuleFor(x => x.PostId).NotEmpty();
    }
}

