using Dawn;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record DocTypeSetDefaultDrafterCommand(string DocTypeId, string UserOftaId): IRequest, IDocTypeKey, IUserOftaKey;

public class DocTypeSetDefaultDrafterHandler: IRequestHandler<DocTypeSetDefaultDrafterCommand>
{
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public DocTypeSetDefaultDrafterHandler(IDocTypeBuilder builder, IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(DocTypeSetDefaultDrafterCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.DocTypeId, y => y.IsNotEmpty())
            .Member(x => x.UserOftaId, y => y.IsNotEmpty());
        
        // BUILD
        var agg = _builder
            .Load(request)
            .DefaultDrafter(request)
            .Build();
        
        // WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}