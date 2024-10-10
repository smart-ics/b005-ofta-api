using Dawn;
using MediatR;
using Ofta.Application.DocContext.DraftOrderAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.DocContext.DraftOrderAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.DocContext.DraftOrderAgg.UseCases;

public record DraftOrderKlaimBpjsEvent(
    DraftOrderModel Agg,
    DraftOrderKlaimBpjsCommand Command
) : INotification;
    
public record DraftOrderKlaimBpjsCommand(
    string DocTypeId,
    string RequesterUserId,
    string DrafterUserId,
    string KlaimBpjsId
) : IRequest, IDocTypeKey, IKlaimBpjsKey;

public class DraftOrderKlaimBpjsHandler: IRequestHandler<DraftOrderKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IDraftOrderBuilder _builder;
    private readonly IDraftOrderWriter _writer;
    private readonly IMediator _mediator;
    private const string CONTEXT_NAME = "Klaim BPJS";

    public DraftOrderKlaimBpjsHandler(IKlaimBpjsBuilder klaimBpjsBuilder, IDraftOrderBuilder builder, IDraftOrderWriter writer, IMediator mediator)
    {
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _builder = builder;
        _writer = writer;
        _mediator = mediator;
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
        _mediator.Publish(new DraftOrderKlaimBpjsEvent(aggregate, request), CancellationToken.None);
        return Task.FromResult(Unit.Value);
    }
}