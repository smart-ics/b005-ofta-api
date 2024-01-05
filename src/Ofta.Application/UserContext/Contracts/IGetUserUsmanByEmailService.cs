// using JetBrains.Annotations;
// using Nuna.Lib.CleanArchHelper;
// #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//
// namespace Ofta.Application.UserContext.Contracts;
//
// public interface IGetUserUsmanByEmailService
//     : INunaService<GetUserUsmanByEmailResponse, string>
// {
// }
//
//
// [PublicAPI]
// public class GetUserUsmanByEmailResponse
// {
//     public string pegId { get; set; }
//     public string userName { get; set; }
//     public string creationDate { get; set; }
//     public string email { get; set; }
//     public bool isApproved { get; set; }
//     public string approvalDate { get; set; }
//     public string expiredDate { get; set; }
//     public int tokenLifeTime { get; set; }
//     public List<ListRole> listRole { get; set; }
//     public List<ListLogin> listLogin { get; set; }
//     public List<object> listLayanan { get; set; }
//     public List<ListTenant> listTenant { get; set; }
// }
//
// public class ListLogin
// {
//     public string appId { get; set; }
//     public string userLogin { get; set; }
// }
//
// public class ListRole
// {
//     public string role { get; set; }
// }
//
// public class ListTenant
// {
//     public string tenantId { get; set; }
//     public string tenantName { get; set; }
// }
