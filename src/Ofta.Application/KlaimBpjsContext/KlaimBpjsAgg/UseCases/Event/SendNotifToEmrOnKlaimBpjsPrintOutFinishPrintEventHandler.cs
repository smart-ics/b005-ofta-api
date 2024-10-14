using MediatR;
using Newtonsoft.Json;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class SendNotifToEmrOnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IAppSettingService _appSettingService;
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IUserBuilder _userBuilder;
    private readonly IListResumeService _listResumeService;
    private readonly ISendNotifToEmrService _sendNotifToEmrService;

    public SendNotifToEmrOnKlaimBpjsPrintOutFinishPrintEventHandler(IAppSettingService appSettingService, IKlaimBpjsBuilder klaimBpjsBuilder, IUserBuilder userBuilder, IListResumeService listResumeService, ISendNotifToEmrService sendNotifToEmrService)
    {
        _appSettingService = appSettingService;
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _userBuilder = userBuilder;
        _listResumeService = listResumeService;
        _sendNotifToEmrService = sendNotifToEmrService;
    }

    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var listResume = _listResumeService.Execute(notification.Agg.RegId);
        var resumeMedis = listResume.FirstOrDefault(x => x.ResumeId == notification.Command.PrintOutReffId);
        if (resumeMedis is null)
            return Task.CompletedTask;

        var klaimBpjs = _klaimBpjsBuilder
            .Load(notification.Agg)
            .Build();
        
        var docType = klaimBpjs.ListDocType.First(x => x.ListPrintOut.Any(y => y.PrintOutReffId == notification.Command.PrintOutReffId));
        
        var user = _userBuilder
            .Load(new UserOftaModel(docType.DrafterUserId))
            .Build();

        var externalSistem = ExternalSystemHelper.GetDestination(docType);
        if (externalSistem is not UserTypeEnum.EMR)
            return Task.CompletedTask;
        
        var toUser = user.ListUserMapping.FirstOrDefault(x => x.PegId == resumeMedis.DokterId && x.UserType == externalSistem);
        if (toUser is null)
            return Task.CompletedTask;

        var messageObj = new
        {
            url = _appSettingService.OftaMyDocWebUrl,
            docTypeId = docType.DocTypeId,
            docTypeName = docType.DocTypeName,
            regId = klaimBpjs.RegId,
            userOfta = user.Email,
            userOftaName = user.UserOftaName,
            userOftaId = user.UserOftaId,
            keterangan = $"Request Sign {docType.DocTypeName}",
        };
        
        var messageJsonString = JsonConvert.SerializeObject(messageObj);
        var reffId = notification.Command.PrintOutReffId;
        
        var reqObj = new EmrNotificationModel(toUser.UserMappingId, messageJsonString, reffId);
        _sendNotifToEmrService.Execute(reqObj);
        return Task.CompletedTask;
    }
}