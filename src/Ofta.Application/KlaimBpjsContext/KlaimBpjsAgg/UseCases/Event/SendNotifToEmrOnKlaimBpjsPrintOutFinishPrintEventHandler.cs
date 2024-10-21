using MediatR;
using Newtonsoft.Json;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class SendNotifToEmrOnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IAppSettingService _appSettingService;
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly ISendNotifToEmrService _sendNotifToEmrService;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IUserOftaMappingDal _userOftaMappingDal;

    public SendNotifToEmrOnKlaimBpjsPrintOutFinishPrintEventHandler(IAppSettingService appSettingService, IKlaimBpjsBuilder klaimBpjsBuilder, ISendNotifToEmrService sendNotifToEmrService, IUserOftaDal userOftaDal, IUserOftaMappingDal userOftaMappingDal)
    {
        _appSettingService = appSettingService;
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _sendNotifToEmrService = sendNotifToEmrService;
        _userOftaDal = userOftaDal;
        _userOftaMappingDal = userOftaMappingDal;
    }

    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var klaimBpjs = _klaimBpjsBuilder
            .Load(notification.Agg)
            .Build();
        
        var docType = klaimBpjs.ListDocType.First(x => x.ListPrintOut.Any(y => y.PrintOutReffId == notification.Command.PrintOutReffId));
        var externalSistem = ExternalSystemHelper.GetDestination(docType);
        if (externalSistem is not UserTypeEnum.EMR)
            return Task.CompletedTask;
        
        var userSigns = JsonConvert.DeserializeObject<UserSignee>(notification.Command.User);
        if (userSigns.UserSign1 != string.Empty)
            SendNotifToEmr(klaimBpjs,  docType, userSigns.UserSign1, notification.Command.PrintOutReffId);
        
        if (userSigns.UserSign2 != string.Empty)
            SendNotifToEmr(klaimBpjs,  docType, userSigns.UserSign2, notification.Command.PrintOutReffId);
        
        if (userSigns.UserSign3 != string.Empty)
            SendNotifToEmr(klaimBpjs,  docType, userSigns.UserSign3, notification.Command.PrintOutReffId);
        
        return Task.CompletedTask;
    }
    
    private void SendNotifToEmr(KlaimBpjsModel klaimBpjs, KlaimBpjsDocTypeModel docType, string user, string reffId)
    {
        var userOftaMapping = _userOftaMappingDal
            .ListData(user)
            .FirstOrDefault(x => x.UserType == UserTypeEnum.EMR);

        if (userOftaMapping is null)
            return;

        var userOfta = _userOftaDal.GetData(userOftaMapping);
        if (userOfta is null)
            return;
        
        var messageObj = new
        {
            url = _appSettingService.OftaMyDocWebUrl,
            docTypeId = docType.DocTypeId,
            docTypeName = docType.DocTypeName,
            regId = klaimBpjs.RegId,
            userOfta = userOfta.Email,
            userOftaName = userOfta.UserOftaName,
            userOftaId = userOfta.UserOftaId,
            keterangan = $"Request Sign {docType.DocTypeName}",
        };
        
        var messageJsonString = JsonConvert.SerializeObject(messageObj);
        var reqObj = new EmrNotificationModel(userOftaMapping.UserMappingId, messageJsonString, reffId);
        _sendNotifToEmrService.Execute(reqObj);
    }
}