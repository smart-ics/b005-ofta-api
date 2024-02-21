using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record CreateKlaimBpjsCommand(string OrderKlaimBpjsId, string UserOftaId) 
    : IRequest<CreateKlaimBpjsResponse>;

public record CreateKlaimBpjsResponse(string DocId);

public class CreateKlaimBpjsCommandHandler : IRequestHandler<CreateKlaimBpjsCommand, CreateKlaimBpjsResponse>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;

    public CreateKlaimBpjsCommandHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<CreateKlaimBpjsResponse> Handle(CreateKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
