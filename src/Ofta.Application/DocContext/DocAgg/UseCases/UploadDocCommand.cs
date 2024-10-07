using Dawn;
using MediatR;
using Nuna.Lib.DataTypeExtension;
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
    private readonly IReqSignToSignProviderService _reqSignToSignProviderService;

    public UploadDocHandler(IDocBuilder builder,
        IDocWriter writer, 
        ISendToSignProviderService sendToSignProviderService,
        IReqSignToSignProviderService reqSignToSignProvider)
    {
        _builder = builder;        
        _writer = writer;
        _sendToSignProviderService = sendToSignProviderService;
        _reqSignToSignProviderService = reqSignToSignProvider;
    }

    public Task<Unit> Handle(UploadDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());
        var aggregate = _builder
            .Load(request)
            .Build();

        //  BUILD
        var uploadedDocId = aggregate.UploadedDocId;

        if (uploadedDocId.IsNullOrEmpty())
        {
            var sendToSignProviderRequest = new SendToSignProviderRequest(aggregate);
            var sendToSignProviderResponse = _sendToSignProviderService.Execute(sendToSignProviderRequest);
            uploadedDocId = sendToSignProviderResponse.UploadedDocId ?? string.Empty;
        }
        
        var reqSignToSignProviderRequest = new ReqSignToSignProviderRequest(aggregate, uploadedDocId);
        var reqSignToSignProviderResponse = _reqSignToSignProviderService.Execute(reqSignToSignProviderRequest);

        if (reqSignToSignProviderResponse?.Signees != null)
        {
            foreach (var updatedSignee in reqSignToSignProviderResponse.Signees)
            {
                var originalSignee = aggregate.ListSignees
                    .FirstOrDefault(s => s.UserOftaId == updatedSignee.UserOftaId || s.Email == updatedSignee.Email);

                if (originalSignee != null)
                {
                    originalSignee.SignUrl = updatedSignee.SignUrl;
                }
            }
        }

        aggregate = _builder
            .Attach(aggregate)
            .AddJurnal(DocStateEnum.Uploaded, string.Empty)
            .UploadedDocId(uploadedDocId)
            .Build();


        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}