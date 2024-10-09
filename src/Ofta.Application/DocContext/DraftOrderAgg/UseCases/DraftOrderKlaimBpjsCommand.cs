using Dawn;
using MediatR;
using Ofta.Application.DocContext.DraftOrderAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.DocContext.DraftOrderAgg.UseCases;

public record DraftOrderKlaimBpjsCommand(
    string DocTypeId,
    string RequesterUserId,
    string DrafterUserId,
    string KlaimBpjsId
): IRequest, IDocTypeKey, IKlaimBpjsKey;

public class DraftOrderKlaimBpjsHandler: IRequestHandler<DraftOrderKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IDraftOrderBuilder _builder;
    private readonly IDraftOrderWriter _writer;
    private const string CONTEXT_NAME = "Klaim BPJS";

    public DraftOrderKlaimBpjsHandler(IKlaimBpjsBuilder klaimBpjsBuilder, IDraftOrderBuilder builder, IDraftOrderWriter writer)
    {
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _builder = builder;
        _writer = writer;
    }
    
    public Task<Unit> Handle(DraftOrderKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty())
            .Member(x => x.RequesterUserId, y => y.NotEmpty())
            .Member(x => x.DrafterUserId, y => y.NotEmpty())
            .Member(x => x.KlaimBpjsId, y => y.NotEmpty());
        
        // BUILD
        var klaimBpjs = _klaimBpjsBuilder
            .Load(request)
            .Build();
        
        var aggregate = _builder
            .Create()
            .DocType(request)
            .RequesterUser(request.RequesterUserId)
            .DrafterUser(request.DrafterUserId)
            .Context(CONTEXT_NAME, klaimBpjs.KlaimBpjsId)
            .Build();
        
        // WRITE
        _ = _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}