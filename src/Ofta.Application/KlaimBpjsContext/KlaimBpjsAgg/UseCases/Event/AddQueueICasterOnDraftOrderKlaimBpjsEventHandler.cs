using MediatR;
using Ofta.Application.DocContext.DraftOrderAgg.UseCases;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.PrintOutContext.ICasterAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueICasterOnDraftOrderKlaimBpjsEventHandler: INotificationHandler<DraftOrderKlaimBpjsEvent>
{
    private readonly IUserBuilder _userBuilder;
    private readonly ISendToICasterService _sendToICasterService;

    public AddQueueICasterOnDraftOrderKlaimBpjsEventHandler(IUserBuilder userBuilder, ISendToICasterService sendToICasterService)
    {
        _userBuilder = userBuilder;
        _sendToICasterService = sendToICasterService;
    }

    public Task Handle(DraftOrderKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var fromUser = notification.Agg.RequesterUserId;

        var userOfta = _userBuilder
            .Load(new UserOftaModel(notification.Agg.DrafterUserId))
            .Build();

        var externalSistem = ExternalSystemHelper.GetDestination(notification.Agg);
        if (externalSistem is not UserTypeEnum.EMR)
            return Task.CompletedTask;
        
        var toUser = userOfta.ListUserMapping.FirstOrDefault(x => x.UserType == externalSistem);
        if (fromUser == string.Empty || toUser is null)
            return Task.CompletedTask;

        var reqObj = new ICasterEmrModel(fromUser, toUser.UserMappingId);
        _sendToICasterService.Execute(reqObj);
        return Task.CompletedTask;
    }
}