namespace Ofta.Domain.UserContext;

public class UserModel : IUserKey
{
    public string PegId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public DateTime ExpiredDate { get; set; }
}