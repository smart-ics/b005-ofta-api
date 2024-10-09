// using FluentValidation;
// using MediatR;
// using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
// using Ofta.Application.UserContext.UserOftaAgg.Workers;
// using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
//
// namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
//
// public record KlaimBpjsOrderDocCommand(string KlaimBpjsId): IRequest, IKlaimBpjsKey;
//
// public class KlaimBpjsOrderDocHandler: IRequestHandler<KlaimBpjsOrderDocCommand>
// {
//     private readonly IKlaimBpjsBuilder _builder;
//     private readonly IUserBuilder _userBuilder;
//     private readonly IMediator _mediator;
//     private readonly IValidator<KlaimBpjsOrderDocCommand> _guard;
//
//     public KlaimBpjsOrderDocHandler(IKlaimBpjsBuilder builder, 
//         IUserBuilder userBuilder, 
//         IMediator mediator, 
//         IValidator<KlaimBpjsOrderDocCommand> guard)
//     {
//         _builder = builder;
//         _userBuilder = userBuilder;
//         _mediator = mediator;
//         _guard = guard;
//     }
//
//     public Task<Unit> Handle(KlaimBpjsOrderDocCommand request, CancellationToken cancellationToken)
//     {
//         // GUARD
//         var guardResult = _guard.Validate(request);
//         if (!guardResult.IsValid)
//             throw new ValidationException(guardResult.Errors);
//
//         // BUILD
//         var agg = _builder
//             .Load(request)
//             .Build();
//
//         var user = _userBuilder
//             .Load(agg)
//             .Build();
//         
//         // WRITE
//         _mediator.Publish(new KlaimBpjsOrderCreateIncompleteDocEvent(user), CancellationToken.None);
//         return Task.FromResult(Unit.Value);
//     }
// }
//
// public class OrderCreateIncompleteDocKlaimBpjsGuard : AbstractValidator<KlaimBpjsOrderDocCommand>
// {
//     public OrderCreateIncompleteDocKlaimBpjsGuard()
//     {
//         RuleFor(x => x.KlaimBpjsId).NotEmpty();
//     }
// }
//
// public record KlaimBpjsOrderCreateIncompleteDocEvent(
//     KlaimBpjsOrderDocCommand Command,
//     KlaimBpjsModel Aggregate) : INotification;
//
