using MediatR;
using Newtonsoft.Json;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.RegContext.RegAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.RegContext.RegAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class SendNotifToEmrOnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IAppSettingService _appSettingService;
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly ISendNotifToEmrService _sendNotifToEmrService;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IUserOftaMappingDal _userOftaMappingDal;
    private readonly IDocBuilder _docBuilder;
    private readonly IGetRegService _getRegService;

    public SendNotifToEmrOnKlaimBpjsPrintOutFinishPrintEventHandler(IAppSettingService appSettingService, IKlaimBpjsBuilder klaimBpjsBuilder, ISendNotifToEmrService sendNotifToEmrService, IUserOftaDal userOftaDal, IUserOftaMappingDal userOftaMappingDal, IDocBuilder docBuilder, IGetRegService getRegService)
    {
        _appSettingService = appSettingService;
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _sendNotifToEmrService = sendNotifToEmrService;
        _userOftaDal = userOftaDal;
        _userOftaMappingDal = userOftaMappingDal;
        _docBuilder = docBuilder;
        _getRegService = getRegService;
    }

    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var klaimBpjs = _klaimBpjsBuilder
            .Load(notification.Agg)
            .Build();
        
        var pasien = _getRegService.Execute(new RegModel(klaimBpjs.RegId));
        var docType = klaimBpjs.ListDocType.First(x => x.ListPrintOut.Any(y => y.PrintOutReffId == notification.Command.PrintOutReffId));
        var docId = docType.ListPrintOut.First(x => x.PrintOutReffId == notification.Command.PrintOutReffId).DocId;
        var doc = _docBuilder.Load(new DocModel(docId)).Build();
        
        var externalSistem = ExternalSystemHelper.GetDestination(docType);
        if (externalSistem is not UserTypeEnum.EMR)
            return Task.CompletedTask;
        
        var userSigns = JsonConvert.DeserializeObject<UserSignee>(notification.Command.User);
        if (userSigns.UserSign1 != string.Empty)
            SendNotifToEmr(klaimBpjs, docType, doc, pasien, userSigns.UserSign1, notification.Command.PrintOutReffId);
        
        if (userSigns.UserSign2 != string.Empty)
            SendNotifToEmr(klaimBpjs, docType, doc, pasien, userSigns.UserSign2, notification.Command.PrintOutReffId);
        
        if (userSigns.UserSign3 != string.Empty)
            SendNotifToEmr(klaimBpjs, docType, doc, pasien, userSigns.UserSign3, notification.Command.PrintOutReffId);
        
        return Task.CompletedTask;
    }
    
    private void SendNotifToEmr(KlaimBpjsModel klaimBpjs, KlaimBpjsDocTypeModel docType, DocModel doc, RegModel pasien, string user, string reffId)
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
            docId = doc.DocId,
            uploadedDocId = doc.UploadedDocId,
            regId = klaimBpjs.RegId,
            pasienId = pasien?.PasienId,
            pasienName = pasien?.PasienName,
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