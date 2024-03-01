using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record UploadDocCommand(string DocId) : IRequest, IDocKey;

public class UploadDocHandler : IRequestHandler<UploadDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly ISendToSignProviderService _sendToSignProviderService;

    public UploadDocHandler(IDocBuilder builder,
        IDocWriter writer, 
        ISendToSignProviderService sendToSignProviderService)
    {
        _builder = builder;        
        _writer = writer;
        _sendToSignProviderService = sendToSignProviderService;
    }

    public Task<Unit> Handle(UploadDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());
        var aggregate = _builder
            .Load(request)
            .Build();
        if (aggregate.DocState != DocStateEnum.Submited)
            throw new ArgumentException("Upload failed: DocState should be Submited");

        
        //  BUILD
        var sendToSignProviderRequest = new SendToSignProviderRequest(aggregate);
        var sendToSignProviderResponse = _sendToSignProviderService.Execute(sendToSignProviderRequest);
        aggregate = _builder
            .Attach(aggregate)
            .AddJurnal(DocStateEnum.Uploaded, string.Empty)
            .UploadedDocId(sendToSignProviderResponse.UploadedDocId)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}