using MediatR;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;

public class UpdateEmrResumeSignStateOnReceiveCallbackSignStatus: INotificationHandler<ReceiveCallbackSignStatusEvent>
{
    private readonly IDocBuilder _builder;
    private readonly IFinishSignEmrResumeService _resumeService;

    public UpdateEmrResumeSignStateOnReceiveCallbackSignStatus(IDocBuilder builder, IFinishSignEmrResumeService resumeService)
    {
        _builder = builder;
        _resumeService = resumeService;
    }

    public Task Handle(ReceiveCallbackSignStatusEvent notification, CancellationToken cancellationToken)
    {
        notification.Agg.ListDoc.ForEach(callbackDoc =>
        {
            var doc = _builder
                .Load(callbackDoc.UploadedDocId)
                .Build();

            if (callbackDoc.UserSignState == SignStateEnum.Signed)
            {
                switch (doc.DocTypeId)
                {
                    case "DTX05":
                        _resumeService.Execute(new FinishSignEmrResumeRequest(doc.DocId));
                        break;
                    
                    case "DTX14":
                        _resumeService.Execute(new FinishSignEmrResumeRequest(doc.DocId));
                        break;
                }
            }
        });
        
        return Task.CompletedTask;
    }
}