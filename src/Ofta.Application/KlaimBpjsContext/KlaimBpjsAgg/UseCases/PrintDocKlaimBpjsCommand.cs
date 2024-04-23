using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record PrintDocKlaimBpjsCommand(string KlaimBpjsId, int NoUrut) : IRequest, IKlaimBpjsKey;

public class PrintDocKlaimBpjsHandler : IRequestHandler<PrintDocKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<PrintDocKlaimBpjsCommand> _guard;
    private readonly IMediator _mediator;

    public PrintDocKlaimBpjsHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<PrintDocKlaimBpjsCommand> guard, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
    }

    public Task<Unit> Handle(PrintDocKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  BUILD
        var klaimBpjs = _builder
            .Load(request)
            .Build();
        var doc = klaimBpjs.ListDocType.FirstOrDefault(x => x.NoUrut == request.NoUrut);
        if (doc is null)
            throw new ArgumentException("Document not found");
        doc.PrintState = PrintStateEnum.Queued;
        klaimBpjs = _builder
            .Attach(klaimBpjs)
            .AddEvent(KlaimBpjsStateEnum.InProgress, $"Print {doc.DocTypeName}")
            .Build();

        //  WRITE
        _writer.Save(klaimBpjs);
        _mediator.Publish(new PrintedDocKlaimBpjsEvent(klaimBpjs, request), cancellationToken);
        return Task.FromResult(Unit.Value);
    }
}

public record PrintedDocKlaimBpjsEvent(
    KlaimBpjsModel Aggregate,
    PrintDocKlaimBpjsCommand Command) : INotification;

public class PrintDocKlaimBpjsCommandValidator : AbstractValidator<PrintDocKlaimBpjsCommand>
{
    public PrintDocKlaimBpjsCommandValidator()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.NoUrut).GreaterThan(0);
    }
}
