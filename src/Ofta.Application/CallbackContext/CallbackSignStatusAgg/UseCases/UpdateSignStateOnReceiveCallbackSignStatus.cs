using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;

public class UpdateSignStateOnReceiveCallbackSignStatus: INotificationHandler<ReceiveCallbackSignStatusEvent>
{
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;

    public UpdateSignStateOnReceiveCallbackSignStatus(IDocBuilder docBuilder, IDocWriter docWriter)
    {
        _docBuilder = docBuilder;
        _docWriter = docWriter;
    }

    public Task Handle(ReceiveCallbackSignStatusEvent notification, CancellationToken cancellationToken)
    {
        notification.Agg.ListDoc.ForEach(callbackDoc =>
        {
            var doc = _docBuilder
                .Load(callbackDoc.UploadedDocId)
                .Build();

            var signee = doc.ListSignees.FirstOrDefault(x => x.UserOftaId == notification.Agg.UserOftaId);
            if (signee is not null)
            {
                signee.SignedDate = notification.Agg.CallbackDate;
                signee.SignState = callbackDoc.UserSignState;
                doc = _docBuilder
                    .Attach(doc)
                    .AddJurnal(DocStateEnum.Signed, signee.Email)
                    .Build();
                
                if (doc.ListSignees.Count > 1)
                {
                    var index = doc.ListSignees.FindIndex(x => x.Email == signee.Email);
                    if (index >= 0 && index != doc.ListSignees.Count - 1)
                        doc.ListSignees.ElementAt(index + 1).IsHidden = signee.SignState != SignStateEnum.Signed;
                }
            }

            _ = _docWriter.Save(doc);
        });
        
        return Task.CompletedTask;
    }
}