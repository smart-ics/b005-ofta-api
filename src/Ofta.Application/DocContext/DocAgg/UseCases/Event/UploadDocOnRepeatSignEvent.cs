using Dawn;
using MediatR;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases.Event;

public class  UploadDocOnRepeatSignEventHandler
    : INotificationHandler<RepeatSignDocEvent>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly ISendToSignProviderService _sendToSignProviderService;
    private readonly IReqSignToSignProviderService _reqSignToSignProviderService;

    public UploadDocOnRepeatSignEventHandler(IDocBuilder builder,
                                             IDocWriter writer,
                                             ISendToSignProviderService sendToSignProviderService,
                                             IReqSignToSignProviderService reqSignToSignProvider)
    {
        _builder = builder;
        _writer = writer;
        _sendToSignProviderService = sendToSignProviderService;
        _reqSignToSignProviderService = reqSignToSignProvider;
    }

    public Task Handle(RepeatSignDocEvent notification, CancellationToken cancellationToken)
    {

        var agg = notification.Agg;
        var cmd = notification.Command;

        //  GUARD
        Guard.Argument(() => agg).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());

        //  BUILD
        var uploadedDocId = agg.UploadedDocId;

        if (uploadedDocId.IsNullOrEmpty())
        {
            var sendToSignProviderRequest = new SendToSignProviderRequest(agg);
            var sendToSignProviderResponse = _sendToSignProviderService.Execute(sendToSignProviderRequest);
            uploadedDocId = sendToSignProviderResponse.UploadedDocId ?? string.Empty;
        }

        var reqSignToSignProviderRequest = new ReqSignToSignProviderRequest(agg, uploadedDocId);
        var reqSignToSignProviderResponse = _reqSignToSignProviderService.Execute(reqSignToSignProviderRequest);

        if (reqSignToSignProviderResponse?.Signees != null)
        {
            foreach (var updatedSignee in reqSignToSignProviderResponse.Signees)
            {
                var originalSignee = agg.ListSignees
                    .FirstOrDefault(s => s.UserOftaId == updatedSignee.UserOftaId || s.Email == updatedSignee.Email);

                if (originalSignee != null)
                {
                    originalSignee.SignUrl = updatedSignee.SignUrl;
                }
            }
        }

        agg = _builder
            .Attach(agg)
            .AddJurnal(DocStateEnum.Uploaded, string.Empty)
            .UploadedDocId(uploadedDocId)
            .Build();


        //  WRITE
        _writer.Save(agg);
        return Task.CompletedTask;
    }

}
