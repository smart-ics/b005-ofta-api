using Dawn;
using MediatR;
using Newtonsoft.Json;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.PrintOutContext.ICasterAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsRequestSignCommand(string KlaimBpjsId, string SignUserId, string PrintOutReffId): IRequest, IKlaimBpjsKey;

public class KlaimBpjsRequestSignHandler: IRequestHandler<KlaimBpjsRequestSignCommand>
{
    private readonly IAppSettingService _appSettingService;
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IDocBuilder _docBuilder;
    private readonly IUserBuilder _userBuilder;
    private readonly ISendToICasterService _sendToICasterService;
    private readonly ISendNotifToEmrService _sendNotifToEmrService;

    public KlaimBpjsRequestSignHandler(IAppSettingService appSettingService, IKlaimBpjsBuilder klaimBpjsBuilder, IDocBuilder docBuilder, IUserBuilder userBuilder, ISendToICasterService sendToICasterService, ISendNotifToEmrService sendNotifToEmrService)
    {
        _appSettingService = appSettingService;
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _docBuilder = docBuilder;
        _userBuilder = userBuilder;
        _sendToICasterService = sendToICasterService;
        _sendNotifToEmrService = sendNotifToEmrService;
    }

    public Task<Unit> Handle(KlaimBpjsRequestSignCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.KlaimBpjsId, y => y.NotEmpty())
            .Member(x => x.SignUserId, y => y.NotEmpty())
            .Member(x => x.PrintOutReffId, y => y.NotEmpty());
        
        // BUILD
        var klaimBpjs = _klaimBpjsBuilder
            .Load(request)
            .Build();
        
        var docType = klaimBpjs.ListDocType.First(x => x.ListPrintOut.Any(y => y.PrintOutReffId == request.PrintOutReffId));
        var docId = docType.ListPrintOut.First(x => x.PrintOutReffId == request.PrintOutReffId).DocId;
        var doc = _docBuilder.Load(new DocModel(docId)).Build();
        
        var externalSistem = ExternalSystemHelper.GetDestination(docType);
        if (externalSistem is not UserTypeEnum.EMR)
            return Task.FromResult(Unit.Value);

        var userOfta = _userBuilder
            .Load(new UserOftaModel(request.SignUserId))
            .Build();

        var userEmr = userOfta.ListUserMapping.FirstOrDefault(x => x.UserType == UserTypeEnum.EMR);
        if (userEmr is null)
            return Task.FromResult(Unit.Value);
        
        var messageObj = new
        {
            url = _appSettingService.OftaMyDocWebUrl,
            docTypeId = docType.DocTypeId,
            docTypeName = docType.DocTypeName,
            docId = doc.DocId,
            uploadedDocId = doc.UploadedDocId,
            regId = klaimBpjs.RegId,
            userOfta = userOfta.Email,
            userOftaName = userOfta.UserOftaName,
            userOftaId = userOfta.UserOftaId,
            keterangan = $"Request Sign {docType.DocTypeName}",
        };
        
        var messageJsonString = JsonConvert.SerializeObject(messageObj);
        
        // EXECUTE
        var emrNotification = new EmrNotificationModel(userEmr.UserMappingId, messageJsonString, request.PrintOutReffId);
        _sendNotifToEmrService.Execute(emrNotification);
        
        var iCasterNotification = new ICasterEmrModel(userEmr.UserOftaId, userEmr.UserMappingId);
        _sendToICasterService.Execute(iCasterNotification);

        return Task.FromResult(Unit.Value);
    }
}