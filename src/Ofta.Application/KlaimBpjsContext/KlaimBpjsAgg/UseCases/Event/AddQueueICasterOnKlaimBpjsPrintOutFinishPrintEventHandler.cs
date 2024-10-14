using MediatR;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.PrintOutContext.ICasterAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueICasterOnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IKlaimBpjsBuilder _klaimBpjsBuilder;
    private readonly IUserBuilder _userBuilder;
    private readonly IListResumeService _listResumeService;
    private readonly ISendToICasterService _sendToICasterService;

    public AddQueueICasterOnKlaimBpjsPrintOutFinishPrintEventHandler(IKlaimBpjsBuilder klaimBpjsBuilder, IUserBuilder userBuilder, IListResumeService listResumeService, ISendToICasterService sendToICasterService)
    {
        _klaimBpjsBuilder = klaimBpjsBuilder;
        _userBuilder = userBuilder;
        _listResumeService = listResumeService;
        _sendToICasterService = sendToICasterService;
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
        
        var userEmr = user.ListUserMapping.FirstOrDefault(x => x.PegId == resumeMedis.DokterId && x.UserType == externalSistem);
        if (userEmr is null)
            return Task.CompletedTask;

        var reqObj = new ICasterEmrModel(userEmr.UserOftaId, userEmr.UserMappingId);
        _sendToICasterService.Execute(reqObj);
        return Task.CompletedTask;
    }
}