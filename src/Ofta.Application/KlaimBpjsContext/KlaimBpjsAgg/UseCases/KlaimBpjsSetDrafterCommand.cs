using Dawn;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsSetDrafterCommand(string KlaimBpjsId, string DocTypeId, string UserId): 
    IRequest, IKlaimBpjsKey, IDocTypeKey;
    
public class KlaimBpjsSetDrafterHandler: IRequestHandler<KlaimBpjsSetDrafterCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;

    public KlaimBpjsSetDrafterHandler(IKlaimBpjsBuilder builder, IKlaimBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(KlaimBpjsSetDrafterCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.KlaimBpjsId, y => y.NotEmpty())
            .Member(x => x.DocTypeId, y => y.NotEmpty())
            .Member(x => x.UserId, y => y.NotEmpty());
        
        // BUILD
        var aggregate = _builder
            .Load(request)
            .SetDocTypeDrafter(request, request.UserId)
            .Build();
        
        // WRITE
        _ = _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}