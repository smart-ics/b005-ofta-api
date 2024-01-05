namespace Ofta.Domain.UserContext;

public class UserModel : IUserKey
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool IsVerified { get; set; }
    public DateTime VerifiedDate { get; set; }
    public DateTime ExpiredDate { get; set; }
}