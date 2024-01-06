namespace Ofta.Domain.UserOftaContext;

public class UserOftaModel : IUserOftaKey
{
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
    public string Email { get; set; }
    public bool IsVerified { get; set; }
    public DateTime VerifiedDate { get; set; }
    public DateTime ExpiredDate { get; set; }
}