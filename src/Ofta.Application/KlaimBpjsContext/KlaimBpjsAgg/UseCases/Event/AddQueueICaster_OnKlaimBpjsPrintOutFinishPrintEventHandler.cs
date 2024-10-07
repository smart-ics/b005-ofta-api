using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.PrintOutContext.ICasterAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueICaster_OnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IUserBuilder _userBuilder;
    private readonly IListResumeService _listResumeService;
    private readonly ISendToICasterService _sendToICasterService;

    public AddQueueICaster_OnKlaimBpjsPrintOutFinishPrintEventHandler(IUserBuilder userBuilder, IListResumeService listResumeService, ISendToICasterService sendToICasterService)
    {
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

        var user = _userBuilder
            .Load(notification.Agg)
            .Build();

        var userEmr = user.ListUserMapping.Find(x => x.PegId == resumeMedis.DokterId && x.UserType == UserTypeEnum.EMR);
        if (userEmr is null)
            return Task.CompletedTask;
        
        var reqObj = new ICasterEmrModel()
        {
            FromUser = userEmr.UserOftaId,
            ToUser = userEmr.UserMappingId,
            Message = new MessageEmrModel
            {
                DocType = "resume",
                DocReff = notification.Command.PrintOutReffId,
            }
        };
        
        _sendToICasterService.Execute(reqObj);
        return Task.CompletedTask;
    }
}