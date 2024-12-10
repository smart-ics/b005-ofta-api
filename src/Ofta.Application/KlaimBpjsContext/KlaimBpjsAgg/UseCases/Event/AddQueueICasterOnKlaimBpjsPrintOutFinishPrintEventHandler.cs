/*using MediatR;
using Newtonsoft.Json;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.PrintOutContext.ICasterAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueICasterOnKlaimBpjsPrintOutFinishPrintEventHandler: INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly ISendToICasterService _sendToICasterService;
    private readonly IUserOftaMappingDal _userOftaMappingDal;

    public AddQueueICasterOnKlaimBpjsPrintOutFinishPrintEventHandler(ISendToICasterService sendToICasterService, IUserOftaMappingDal userOftaMappingDal)
    {
        _sendToICasterService = sendToICasterService;
        _userOftaMappingDal = userOftaMappingDal;
    }

    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var userSigns = JsonConvert.DeserializeObject<UserSignee>(notification.Command.User);
        
        if (userSigns.UserSign1 != string.Empty)
            SendNotifToIcaster(userSigns.UserSign1);
        
        if (userSigns.UserSign2 != string.Empty)
            SendNotifToIcaster(userSigns.UserSign2);
        
        if (userSigns.UserSign3 != string.Empty)
            SendNotifToIcaster(userSigns.UserSign3);
        
        return Task.CompletedTask;
    }

    private void SendNotifToIcaster(string user)
    {
        var userOfta = _userOftaMappingDal
            .ListData(user)
            .First();

        if (userOfta is null)
            return;
        
        var reqObj = new ICasterEmrModel(userOfta.UserOftaId, userOfta.UserMappingId);
        _sendToICasterService.Execute(reqObj);
    }
}*/