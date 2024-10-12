using Dawn;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsDocTypeSetDrafterCommand(string KlaimBpjsId, string DocTypeId, string UserOftaId): 
    IRequest, IKlaimBpjsKey, IDocTypeKey, IUserOftaKey;
    
public class KlaimBpjsSetDrafterHandler: IRequestHandler<KlaimBpjsDocTypeSetDrafterCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IUserBuilder _userBuilder;
    private readonly IKlaimBpjsWriter _writer;

    public KlaimBpjsSetDrafterHandler(IKlaimBpjsBuilder builder, IUserBuilder userBuilder, IKlaimBpjsWriter writer)
    {
        _builder = builder;
        _userBuilder = userBuilder;
        _writer = writer;
    }

    public Task<Unit> Handle(KlaimBpjsDocTypeSetDrafterCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.KlaimBpjsId, y => y.NotEmpty())
            .Member(x => x.DocTypeId, y => y.NotEmpty())
            .Member(x => x.UserOftaId, y => y.NotEmpty());
        
        // BUILD
        var user = _userBuilder
            .Load(request)
            .Build();
        
        var aggregate = _builder
            .Load(request)
            .SetDocTypeDrafter(request, user)
            .Build();
        
        // WRITE
        _ = _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}