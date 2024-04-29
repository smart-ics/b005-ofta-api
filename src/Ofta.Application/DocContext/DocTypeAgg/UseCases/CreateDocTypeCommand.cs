using Dawn;
using Mapster;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record CreateDocTypeCommand(string DocTypeName, string DocTypeCode)
    : IRequest<CreateDocTypeResponse>;

public class CreateDocTypeResponse
{
    public string DocTypeId { get; set; }
}

public class CreateDocTypeHandler : IRequestHandler<CreateDocTypeCommand, CreateDocTypeResponse>
{
    private DocTypeModel _aggregate;
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public CreateDocTypeHandler(IDocTypeBuilder builder, 
        IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<CreateDocTypeResponse> Handle(CreateDocTypeCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocTypeName, y => y.NotEmpty());
        
        //  BUILDER
        _aggregate = _builder
            .Create()
            .Name(request.DocTypeName)
            .DocTypeCode(request.DocTypeCode)
            .IsActive(true)
            .Build();
        
        //  WRITE
        var result = _writer.Save(_aggregate);
        var response = result.Adapt<CreateDocTypeResponse>();
        return Task.FromResult(response);
    }
}
