using MediatR;
using Ofta.Application.DocContext.DraftOrderAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.PrintOutContext.ICasterAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueICasterOnDraftOrderKlaimBpjsEventHandler: INotificationHandler<DraftOrderKlaimBpjsEvent>
{
    private readonly ISendToICasterService _sendToICasterService;

    public AddQueueICasterOnDraftOrderKlaimBpjsEventHandler(ISendToICasterService sendToICasterService)
    {
        _sendToICasterService = sendToICasterService;
    }

    public Task Handle(DraftOrderKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var fromUser = notification.Agg.RequesterUserId;
        var toUser = notification.Agg.DrafterUserId;

        if (fromUser == string.Empty || toUser == string.Empty)
            return Task.CompletedTask;

        var reqObj = new ICasterEmrModel(fromUser, toUser);
        _sendToICasterService.Execute(reqObj);
        return Task.CompletedTask;
    }
}