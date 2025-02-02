using Dawn;
using MediatR;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.UseCases;

public record SetToBePrintedBlueprintCommand(string BlueprintId, string DocTypeId): IRequest, IBlueprintKey, IDocTypeKey;

public class SetToBePrintedBlueprintHandler: IRequestHandler<SetToBePrintedBlueprintCommand>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;

    public SetToBePrintedBlueprintHandler(IBlueprintBuilder builder, IBlueprintWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }
    
    public Task<Unit> Handle(SetToBePrintedBlueprintCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.BlueprintId, y => y.NotEmpty())
            .Member(x => x.DocTypeId, y => y.NotEmpty());
        
        // BUILDER
        var blueprint = _builder
            .Load(request)
            .SetToBePrinted(request)
            .Build();
        
        // WRITE
        _writer.Save(blueprint);
        return Task.FromResult(Unit.Value);
    }
}