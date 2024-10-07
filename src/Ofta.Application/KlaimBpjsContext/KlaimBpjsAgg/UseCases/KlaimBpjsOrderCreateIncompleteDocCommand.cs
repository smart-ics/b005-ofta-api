using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsOrderCreateIncompleteDocEvent(UserOftaModel User) : INotification;

public record KlaimBpjsOrderCreateIncompleteDocCommand(string KlaimBpjsId): IRequest, IKlaimBpjsKey;

public class KlaimBpjsOrderCreateIncompleteDocHandler: IRequestHandler<KlaimBpjsOrderCreateIncompleteDocCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IUserBuilder _userBuilder;
    private readonly IMediator _mediator;
    private readonly IValidator<KlaimBpjsOrderCreateIncompleteDocCommand> _guard;

    public KlaimBpjsOrderCreateIncompleteDocHandler(IKlaimBpjsBuilder builder, IUserBuilder userBuilder, IMediator mediator, IValidator<KlaimBpjsOrderCreateIncompleteDocCommand> guard)
    {
        _builder = builder;
        _userBuilder = userBuilder;
        _mediator = mediator;
        _guard = guard;
    }

    public Task<Unit> Handle(KlaimBpjsOrderCreateIncompleteDocCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);

        // BUILD
        var agg = _builder
            .Load(request)
            .Build();

        var user = _userBuilder
            .Load(agg)
            .Build();
        
        // WRITE
        _mediator.Publish(new KlaimBpjsOrderCreateIncompleteDocEvent(user), CancellationToken.None);
        return Task.FromResult(Unit.Value);
    }
}

public class OrderCreateIncompleteDocKlaimBpjsGuard : AbstractValidator<KlaimBpjsOrderCreateIncompleteDocCommand>
{
    public OrderCreateIncompleteDocKlaimBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
    }
}