using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;

public class DownloadDocOnReceiveCallbackSignStatus: INotificationHandler<ReceiveCallbackSignStatusEvent>
{
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;
    private readonly IDownloadPublishedDocFromProviderService _downloadService;

    public DownloadDocOnReceiveCallbackSignStatus(IDocBuilder docBuilder, IDocWriter docWriter, IDownloadPublishedDocFromProviderService downloadService)
    {
        _docBuilder = docBuilder;
        _docWriter = docWriter;
        _downloadService = downloadService;
    }

    public Task Handle(ReceiveCallbackSignStatusEvent notification, CancellationToken cancellationToken)
    {
        notification.Agg.ListDoc.ForEach(callbackDoc =>
        {
            var doc = _docBuilder
                .Load(callbackDoc.UploadedDocId)
                .GenPublishedDocUrl()
                .Build();
            
            var downloadReq = new DownloadPublishedDocFromProviderRequest(
                callbackDoc.DownloadDocUrl, doc.PublishedDocUrl);
            _downloadService.Execute(downloadReq);

            var signee = doc.ListSignees.FirstOrDefault(x => x.UserOftaId == notification.Agg.UserOftaId);
            if (signee is not null)
                doc = _docBuilder
                    .Attach(doc)
                    .UploadedDocUrl(callbackDoc.DownloadDocUrl)
                    .Build();

            _ = _docWriter.Save(doc);
        });
        
        return Task.CompletedTask;
    }
}