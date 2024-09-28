namespace Ofta.Domain.UserContext.UserOftaAgg;

public class UserOftaModel : IUserOftaKey
{
    public UserOftaModel()
    {
    }

    public UserOftaModel(string id) => UserOftaId = id;

    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
    public string Email { get; set; }
    public bool IsVerified { get; set; }
    public DateTime VerifiedDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    
    public List<UserOftaMappingModel> ListUserMapping { get; set; }
}