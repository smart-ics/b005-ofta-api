using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record CreateDocCommand(string DocTypeId, string UserOftaId) :
    IRequest<CreateDocResponse>, IDocTypeKey, IUserOftaKey;

public record CreateDocResponse(string DocId);


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
            .Member(x => x.UserOftaId, y => y.NotEmpty());
        
        //  BUILD
        var aggregate = _docBuilder
            .Create()
            .DocType(request)
            .User(request)
            .AddJurnal(DocStateEnum.Created, string.Empty)
            .Build();
        
        //  WRITE
        aggregate = _docWriter.Save(aggregate);
        var response = new CreateDocResponse(aggregate.DocId);
        return Task.FromResult(response);
    }
}
