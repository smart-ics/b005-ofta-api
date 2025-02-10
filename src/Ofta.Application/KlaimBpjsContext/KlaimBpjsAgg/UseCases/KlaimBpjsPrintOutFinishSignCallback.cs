using Dawn;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsPrintOutFinishSignCallbackCommand(string KlaimBpjsId, string PrintOutReffId, string DocId)
    : IRequest, IKlaimBpjsKey, IDocKey;
    
public class KlaimBpjsPrintOutFinishSignCallbackHandler: IRequestHandler<KlaimBpjsPrintOutFinishSignCallbackCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;

    public KlaimBpjsPrintOutFinishSignCallbackHandler(IKlaimBpjsBuilder builder, IKlaimBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(KlaimBpjsPrintOutFinishSignCallbackCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.KlaimBpjsId, y => y.NotEmpty())
            .Member(x => x.PrintOutReffId, y => y.NotEmpty())
            .Member(x => x.DocId, y => y.NotEmpty());

        // BUILD
        var agg = _builder
            .Load(request)
            .FinishPrintOut(request.PrintOutReffId)
            .AddSignedDocToPrintOut(request.PrintOutReffId, request)
            .UpdateStateCompleted()
            .Build();

        // WRITE
        _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}