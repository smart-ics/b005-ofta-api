using Dawn;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record CreateDocTypeCommand(string DocTypeName)
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
    private readonly ILogger<CreateDocTypeHandler> _logger;

    public CreateDocTypeHandler(IDocTypeBuilder builder, 
        IDocTypeWriter writer, 
        ILogger<CreateDocTypeHandler> logger)
    {
        _builder = builder;
        _writer = writer;
        _logger = logger;
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
            .IsActive(true)
            .Build();
        
        _logger.LogTrace("Ini trace log!!!!!");
        _logger.LogDebug("Ini debug log!!!!!");
        _logger.LogInformation("Ini info log!!!!!");
        _logger.LogWarning("Ini warning log!!!!!");
        _logger.LogError("Ini error log!!!!!");
        _logger.LogCritical("Ini critical log!!!!!");
        
        //  WRITE
        var result = _writer.Save(_aggregate);
        var response = result.Adapt<CreateDocTypeResponse>();
        return Task.FromResult(response);

    }
}
