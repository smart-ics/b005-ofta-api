using MediatR;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;

public class UpdateSmassAssesmentSignStateOnReceiveCallbackSignStatus: INotificationHandler<ReceiveCallbackSignStatusEvent>
{
    private readonly IDocBuilder _builder;
    private readonly IFinishSignSmassAssesmentService _assesmentService;

    public UpdateSmassAssesmentSignStateOnReceiveCallbackSignStatus(IDocBuilder builder, IFinishSignSmassAssesmentService assesmentService)
    {
        _builder = builder;
        _assesmentService = assesmentService;
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
                    case "DTX0F":
                        _assesmentService.Execute(new IFinishSignSmassAssesmentRequest(doc.DocId));
                        break;
                }
            }
        });
        
        return Task.CompletedTask;
    }
}