using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record GenDocCommand(string DocId) : IRequest<GenDocResponse>, IDocKey;

public class GenDocResponse
{
    public string RequestedDocUrl { get; set; }
}

public class GenDocHandler : IRequestHandler<GenDocCommand, GenDocResponse>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;

    public GenDocHandler(IDocBuilder builder, 
        IDocWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<GenDocResponse> Handle(GenDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        
        //  BUILD
        var aggregate = _builder
            .Create(request)
            .DocState()
            .Build();
        
        //  WRITE
        _

    }
} 