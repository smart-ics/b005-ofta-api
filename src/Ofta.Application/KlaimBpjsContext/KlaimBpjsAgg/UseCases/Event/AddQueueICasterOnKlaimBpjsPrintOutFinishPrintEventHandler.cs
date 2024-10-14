using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.PrintOutContext.ICasterAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueICasterOnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IUserBuilder _userBuilder;
    private readonly IListResumeService _listResumeService;
    private readonly ISendToICasterService _sendToICasterService;

    public AddQueueICasterOnKlaimBpjsPrintOutFinishPrintEventHandler(IUserBuilder userBuilder, IListResumeService listResumeService, ISendToICasterService sendToICasterService)
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

        var userEmr = user.ListUserMapping.FirstOrDefault(x => x.PegId == resumeMedis.DokterId);
        if (userEmr is null)
            return Task.CompletedTask;

        var reqObj = new ICasterEmrModel(userEmr.UserOftaId, userEmr.UserMappingId);
        _sendToICasterService.Execute(reqObj);
        return Task.CompletedTask;
    }
}