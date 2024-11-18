using Dawn;
using MediatR;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.UseCases;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public record UploadDocBulkSignCommand(string DocId) : IRequest, IDocKey;

public class UploadDocHandler : IRequestHandler<UploadDocBulkSignCommand>
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

    public Task<Unit> Handle(UploadDocBulkSignCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());
        var aggregate = _builder
            .Load(request)
            .Build();

        // BUILD
        var uploadedDocId = aggregate.UploadedDocId;
        if (uploadedDocId.IsNullOrEmpty())
        {
            var sendToSignProviderRequest = new SendToSignProviderRequest(aggregate);
            var sendToSignProviderResponse = _sendToSignProviderService.Execute(sendToSignProviderRequest);
            uploadedDocId = sendToSignProviderResponse.UploadedDocId;
        }

        aggregate = _builder
            .Attach(aggregate)
            .AddJurnal(DocStateEnum.Uploaded, string.Empty)
            .UploadedDocId(uploadedDocId)
            .Build();
        
        // WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}