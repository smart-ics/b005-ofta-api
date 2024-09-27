namespace Ofta.Domain.UserContext.UserOftaAgg;

public class UserOftaMappingModel: IUserOftaKey
{
    public UserOftaMappingModel(string userOftaId, string userMappingId, UserTypeEnum userType)
    {
        UserOftaId = userOftaId;
        UserMappingId = userMappingId;
        UserType = userType;
    }
    
    public string UserOftaId { get; protected set; }
    public string UserMappingId { get; protected set; }
    public UserTypeEnum UserType { get; protected set; }
}