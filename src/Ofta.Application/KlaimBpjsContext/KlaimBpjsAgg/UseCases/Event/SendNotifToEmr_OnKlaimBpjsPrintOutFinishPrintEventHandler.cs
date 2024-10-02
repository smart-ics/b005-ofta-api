using MediatR;
using Newtonsoft.Json;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class SendNotifToEmr_OnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IDocTypeBuilder _docTypeBuilder;
    private readonly IUserBuilder _userBuilder;
    private readonly IListResumeService _listResumeService;
    private readonly ISendNotifToEmrService _sendNotifToEmrService;
    private const string DOC_TYPE_NAME = "Resume Medis"; 

    public SendNotifToEmr_OnKlaimBpjsPrintOutFinishPrintEventHandler(IKlaimBpjsBuilder klaimBpjsBuilder, IDocTypeBuilder docTypeBuilder, IUserBuilder userBuilder, IListResumeService listResumeService, ISendNotifToEmrService sendNotifToEmrService)
    {
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _docTypeBuilder = docTypeBuilder;
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
        
        var user = _userBuilder
            .Load(notification.Agg)
            .Build();

        var docType = klaimBpjs.ListDocType.First(x => x.DocTypeName == DOC_TYPE_NAME); 

        var userEmr = user.ListUserMapping.Find(x => x.PegId == resumeMedis.DokterId && x.UserType == UserTypeEnum.EMR);
        if (userEmr is null)
            return Task.CompletedTask;

        var messageObj = new
        {
            Keterangan = "Request Sign Resume Medis",
            KodeReg = klaimBpjs.RegId,
            DocType = docType.DocTypeId,
            UrlOfta = "ofta.com",
            UserOfta = userEmr.UserOftaId
        };
        
        var messageJsonString = JsonConvert.SerializeObject(messageObj);
        
        var reqObj = new EmrNotificationModel(
            userEmr.UserMappingId,
            messageJsonString,
            notification.Command.PrintOutReffId
        );
        
        _sendNotifToEmrService.Execute(reqObj);
        return Task.CompletedTask;
    }
}