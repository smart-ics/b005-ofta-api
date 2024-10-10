using MediatR;
using Newtonsoft.Json;
using Ofta.Application.DocContext.DraftOrderAgg.UseCases;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class SendNotifToEmrOnDraftOrderKlaimBpjsEventHandler : INotificationHandler<DraftOrderKlaimBpjsEvent>
{
    private readonly IAppSettingService _appSettingService;
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IUserBuilder _userBuilder;
    private readonly ISendNotifToEmrService _sendNotifToEmrService;

    public SendNotifToEmrOnDraftOrderKlaimBpjsEventHandler(IAppSettingService appSettingService, IKlaimBpjsBuilder klaimBpjsBuilder, IUserBuilder userBuilder, ISendNotifToEmrService sendNotifToEmrService)
    {
        _appSettingService = appSettingService;
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _userBuilder = userBuilder;
        _sendNotifToEmrService = sendNotifToEmrService;
    }

    public Task Handle(DraftOrderKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var toUser = notification.Agg.DrafterUserId;

        if (toUser == string.Empty)
            return Task.CompletedTask;

        var klaimBpjs = _klaimBpjsBuilder
            .Load(notification.Command)
            .Build();

        var user = _userBuilder
            .Load(new UserOftaModel(notification.Agg.RequesterUserId))
            .Build();

        var messageObj = new
        {
            url = _appSettingService.OftaMyDocWebUrl,
            docTypeId = notification.Agg.DocTypeId,
            docTypeName = notification.Agg.DocTypeName,
            regId = klaimBpjs.RegId,
            userOfta = user.Email,
            userOftaName = user.UserOftaName,
            userOftaId = user.UserOftaId,
            keterangan = $"Request Create {notification.Agg.DocTypeName}",
        };

        var messageJsonString = JsonConvert.SerializeObject(messageObj);
        var reffId = $"{klaimBpjs.RegId}-{notification.Agg.DocTypeId}";

        var reqObj = new EmrNotificationModel(toUser, messageJsonString, reffId);
        _sendNotifToEmrService.Execute(reqObj);
        return Task.CompletedTask;
    }
}