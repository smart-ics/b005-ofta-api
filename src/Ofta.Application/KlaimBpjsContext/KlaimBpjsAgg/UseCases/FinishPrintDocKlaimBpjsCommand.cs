using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record FinishPrintDocKlaimBpjsCommand(string KlaimBpjsId, string PrintOutReffId, string Base64Content)
    : IRequest, IKlaimBpjsKey;

public class FinishPrintOutKlaimBpjsHandler : IRequestHandler<FinishPrintDocKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<FinishPrintDocKlaimBpjsCommand> _guard;
    private readonly IMediator _mediator;

    public FinishPrintOutKlaimBpjsHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<FinishPrintDocKlaimBpjsCommand> guard, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
    }

    public Task<Unit> Handle(FinishPrintDocKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder.Load(request).Build();
        var itemKlaim = agg.ListDocType.FirstOrDefault(x => x.PrintOutReffId == request.PrintOutReffId);
        if (itemKlaim is null)
            throw new ArgumentException("Document not found");
        itemKlaim.PrintState = PrintStateEnum.Printed;
        
        //  WRITE
        _writer.Save(agg);
        _mediator.Publish(new FinishedPrintDocKlaimBpjsEvent(agg, request), cancellationToken);
        return Task.FromResult(Unit.Value);
    }
}

public record FinishedPrintDocKlaimBpjsEvent(
    KlaimBpjsModel Agg,
    FinishPrintDocKlaimBpjsCommand Command) : INotification;

public class FinishPrintDocKlaimBpjsGuard : AbstractValidator<FinishPrintDocKlaimBpjsCommand>
{
    public FinishPrintDocKlaimBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.PrintOutReffId).NotEmpty();
        RuleFor(x => x.Base64Content).NotEmpty();
    }
}