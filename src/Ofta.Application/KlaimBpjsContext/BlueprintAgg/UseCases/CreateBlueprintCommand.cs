using Dawn;
using MediatR;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.UseCases;

public record CreateBlueprintCommand(string BlueprintName)
    : IRequest<CreateBlueprintResponse>;

public record CreateBlueprintResponse(string BlueprintId);

public class CreateBlueprintHandler : IRequestHandler<CreateBlueprintCommand, CreateBlueprintResponse>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;

    public CreateBlueprintHandler(IBlueprintBuilder builder, 
        IBlueprintWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<CreateBlueprintResponse> Handle(CreateBlueprintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.BlueprintName, y => y.NotEmpty());
        
        //  BUILDER
        var blueprint = _builder
            .Create()
            .Name(request.BlueprintName)
            .Build();
        
        //  WRITE
        var result = _writer.Save(blueprint);
        return Task.FromResult(new CreateBlueprintResponse(result.BlueprintId));
    }
}
