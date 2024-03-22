using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record SetPrintReffIdKlaimBpjsCommand(string KlaimBpjsId, int NoUrut, string PrintReffId) 
    : IRequest, IKlaimBpjsKey;

public class SetPrintReffIdKlaimBpjsHandler : IRequestHandler<SetPrintReffIdKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<SetPrintReffIdKlaimBpjsCommand> _guard;
    private readonly IMediator _mediator;
    public SetPrintReffIdKlaimBpjsHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<SetPrintReffIdKlaimBpjsCommand> guard, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
    }

    public Task<Unit> Handle(SetPrintReffIdKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
            
        //  BUILD
        var agg = _builder
            .Load(request)
            .Build();
        var doc = agg.ListDoc.FirstOrDefault(x => x.NoUrut == request.NoUrut);
        if (doc is null)
            throw new ArgumentException("Document not found");
        
        agg = _builder
            .Attach(agg)
            .PrintReffId(request.NoUrut, request.PrintReffId)
            .AddEvent(KlaimBpjsStateEnum.InProgress, $"Set PrintReffID {doc.DocTypeName} -> {request.PrintReffId}")
            .Build();
        
        _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class SetPrintReffIdKlaimBpjsGuard : AbstractValidator<SetPrintReffIdKlaimBpjsCommand>
{
    public SetPrintReffIdKlaimBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.NoUrut).GreaterThan(0);
        RuleFor(x => x.PrintReffId).NotEmpty();
    }
}

public record SetPrintReffIdKlaimBpjsEvent(
    KlaimBpjsModel Agg,
    SetPrintReffIdKlaimBpjsCommand Command) : INotification;
