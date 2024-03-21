using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record SetPrintReffIdKlaimBpjsCommand(string KlaimBpjsId, int NoUrut, string PrintReffId) 
    : IRequest, IKlaimBpjsKey;

public class SetPrintReffIdKlaimBpjsHandler : IRequestHandler<SetPrintReffIdKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;

    public SetPrintReffIdKlaimBpjsHandler(IKlaimBpjsBuilder builder, IKlaimBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(SetPrintReffIdKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        var agg = _builder
            .Load(request)
            .PrintReffId(request.NoUrut, request.PrintReffId)
            .AddEvent(KlaimBpjsStateEnum.InProgress, $"Set Print Reff ID {request.PrintReffId}")
            .Build();
        _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}
