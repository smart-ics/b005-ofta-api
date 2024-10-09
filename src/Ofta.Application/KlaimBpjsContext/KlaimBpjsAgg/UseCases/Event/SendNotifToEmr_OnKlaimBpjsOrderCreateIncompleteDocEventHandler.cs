// using MediatR;
// using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
// using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
// using Ofta.Domain.UserContext.UserOftaAgg;
//
// namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;
//
// public class
//     SendNotifToEmr_OnKlaimBpjsOrderCreateIncompleteDocEventHandler : INotificationHandler<
//     KlaimBpjsOrderCreateIncompleteDocEvent>
// {
//     private readonly ISendNotifToEmrService _sendNotifToEmrService;
//
//     public SendNotifToEmr_OnKlaimBpjsOrderCreateIncompleteDocEventHandler(ISendNotifToEmrService sendNotifToEmrService)
//     {
//         _sendNotifToEmrService = sendNotifToEmrService;
//     }
//
//     public Task Handle(KlaimBpjsOrderCreateIncompleteDocEvent notification, CancellationToken cancellationToken)
//     {
//         var mappingUser = notification.User.ListUserMapping.Find(x => x.UserType == UserTypeEnum.EMR);
//
//         if (mappingUser is null)
//             return Task.CompletedTask;
//
//         var reqObj = new EmrNotificationModel(mappingUser.UserMappingId, 
//             "Request Create Resume Medis", "-");
//
//         _sendNotifToEmrService.Execute(reqObj);
//         return Task.CompletedTask;
//     }
// }