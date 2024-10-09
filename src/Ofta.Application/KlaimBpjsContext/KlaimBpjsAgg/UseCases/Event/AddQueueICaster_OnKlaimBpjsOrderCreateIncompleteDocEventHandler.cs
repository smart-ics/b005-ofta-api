// using MediatR;
// using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
// using Ofta.Domain.PrintOutContext.ICasterAgg;
// using Ofta.Domain.UserContext.UserOftaAgg;
//
// namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;
//
// public class AddQueueICaster_OnKlaimBpjsOrderCreateIncompleteDocEventHandler: INotificationHandler<KlaimBpjsOrderCreateIncompleteDocEvent>
// {
//     private readonly ISendToICasterService _sendToICasterService;
//
//     public AddQueueICaster_OnKlaimBpjsOrderCreateIncompleteDocEventHandler(ISendToICasterService sendToICasterService)
//     {
//         _sendToICasterService = sendToICasterService;
//     }
//
//     public Task Handle(KlaimBpjsOrderCreateIncompleteDocEvent notification, CancellationToken cancellationToken)
//     {
//         var userEmr = notification.User.ListUserMapping.Find(x => x.UserType == UserTypeEnum.EMR);
//
//         if (userEmr is null)
//             return Task.CompletedTask;
//         
//         var reqObj = new ICasterEmrModel()
//         {
//             FromUser = userEmr.UserOftaId,
//             ToUser = userEmr.UserMappingId,
//             Message = new MessageEmrModel
//             {
//                 DocType = "resume",
//                 DocReff = string.Empty,
//             }
//         };
//         
//         _sendToICasterService.Execute(reqObj);
//         return Task.CompletedTask;
//     }
// }