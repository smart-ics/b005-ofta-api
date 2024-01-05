using Dawn;
using Mapster;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record CreateDocCommand(string DocTypeId, string UserId) :
    IRequest<CreateDocResponse>, IDocTypeKey, IUserKey;

public class CreateDocResponse
{
    public string DocId { get; set; }
}

public class CreateDocHandler : IRequestHandler<CreateDocCommand, CreateDocResponse>
{
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;

    public CreateDocHandler(IDocBuilder docBuilder, 
        IDocWriter docWriter)
    {
        _docBuilder = docBuilder;
        _docWriter = docWriter;
    }

    public Task<CreateDocResponse> Handle(CreateDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty())
            .Member(x => x.UserId, y => y.NotEmpty());
        
        //  BUILD
        var aggregate = _docBuilder
            .Create()
            .DocType(request)
            .User(request)
            .Build();
        
        //  WRITE
        aggregate = _docWriter.Save(aggregate);
        var response = aggregate.Adapt<CreateDocResponse>();
        return Task.FromResult(response);
    }
}
