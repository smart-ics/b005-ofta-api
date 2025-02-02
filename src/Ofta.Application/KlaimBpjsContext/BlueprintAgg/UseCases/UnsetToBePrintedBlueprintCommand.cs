using Dawn;
using MediatR;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.UseCases;

public record UnsetToBePrintedBlueprintCommand(string BlueprintId, string DocTypeId): IRequest, IBlueprintKey, IDocTypeKey;

public class UnsetToBePrintedBlueprintHandler: IRequestHandler<UnsetToBePrintedBlueprintCommand>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;

    public UnsetToBePrintedBlueprintHandler(IBlueprintBuilder builder, IBlueprintWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }
    
    public Task<Unit> Handle(UnsetToBePrintedBlueprintCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.BlueprintId, y => y.NotEmpty())
            .Member(x => x.DocTypeId, y => y.NotEmpty());
        
        // BUILDER
        var blueprint = _builder
            .Load(request)
            .UnsetToBePrinted(request)
            .Build();
        
        // WRITE
        _writer.Save(blueprint);
        return Task.FromResult(Unit.Value);
    }
}