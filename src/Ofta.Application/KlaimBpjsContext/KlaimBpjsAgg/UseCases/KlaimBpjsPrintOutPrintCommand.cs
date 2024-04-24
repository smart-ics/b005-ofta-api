using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsPrintOutPrintCommand(string KlaimBpjsId, string DocTypeId, string ReffId) 
    : IRequest, IKlaimBpjsKey, IDocTypeKey;

public class KlaimBpjsPrintOutPrintHandler : IRequestHandler<KlaimBpjsPrintOutPrintCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<KlaimBpjsPrintOutPrintCommand> _guard;
    private readonly IMediator _mediator;

    public KlaimBpjsPrintOutPrintHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<KlaimBpjsPrintOutPrintCommand> guard, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
    }

    public Task<Unit> Handle(KlaimBpjsPrintOutPrintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  BUILD
        var klaimBpjs = _builder
            .Load(request)
            .PrintDoc(request, request.ReffId)
            .Build();
        
        //  WRITE
        _writer.Save(klaimBpjs);
        _mediator.Publish(new PrintedDocKlaimBpjsEvent(klaimBpjs, request), cancellationToken);
        return Task.FromResult(Unit.Value);
    }
}

public record PrintedDocKlaimBpjsEvent(
    KlaimBpjsModel Aggregate,
    KlaimBpjsPrintOutPrintCommand Command) : INotification;

public class KlaimBpjsPrintOutPrintCommandValidator : AbstractValidator<KlaimBpjsPrintOutPrintCommand>
{
    public KlaimBpjsPrintOutPrintCommandValidator()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.DocTypeId).NotEmpty();
        RuleFor(x => x.ReffId).NotEmpty();
    }
}
