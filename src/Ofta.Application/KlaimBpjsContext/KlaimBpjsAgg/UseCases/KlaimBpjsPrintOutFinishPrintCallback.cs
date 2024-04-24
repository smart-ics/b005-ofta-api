using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsPrintOutFinishPrintCallback(string KlaimBpjsId, string PrintOutReffId, string Base64Content)
    : IRequest, IKlaimBpjsKey;

public class KlaimBpjsPrintOutFinishPrintHandler : IRequestHandler<KlaimBpjsPrintOutFinishPrintCallback>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<KlaimBpjsPrintOutFinishPrintCallback> _guard;
    private readonly IMediator _mediator;

    public KlaimBpjsPrintOutFinishPrintHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<KlaimBpjsPrintOutFinishPrintCallback> guard, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
    }

    public Task<Unit> Handle(KlaimBpjsPrintOutFinishPrintCallback request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .FinishPrintOut(request.PrintOutReffId)
            .Build();
        
        //  WRITE
        _writer.Save(agg);
        _mediator.Publish(new FinishedPrintDocKlaimBpjsEvent(agg, request), cancellationToken);
        return Task.FromResult(Unit.Value);
    }
}

public record FinishedPrintDocKlaimBpjsEvent(
    KlaimBpjsModel Agg,
    KlaimBpjsPrintOutFinishPrintCallback Command) : INotification;

public class FinishPrintDocKlaimBpjsGuard : AbstractValidator<KlaimBpjsPrintOutFinishPrintCallback>
{
    public FinishPrintDocKlaimBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.PrintOutReffId).NotEmpty();
        RuleFor(x => x.Base64Content).NotEmpty();
    }
}