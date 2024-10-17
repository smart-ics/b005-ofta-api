using Mapster.Utils;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.UserOftaAgg.Workers;

public interface IUserBuilder : INunaBuilder<UserOftaModel>
{
    IUserBuilder Create();
    IUserBuilder Load(IUserOftaKey userOftaKey);
    IUserBuilder Load(string email);

    IUserBuilder UserOftaName(string userOftaName);
    IUserBuilder Email(string email);
    IUserBuilder AddUserMapping(string userMappingId, string pegId, string userType);
}
public class UserBuilder : IUserBuilder
{
    private UserOftaModel _aggregate = new();
    private readonly IUserOftaDal _userDal;
    private readonly IUserOftaMappingDal _userOftaMappingDal;

    public UserBuilder(IUserOftaDal userDal, IUserOftaMappingDal userOftaMappingDal)
    {
        _userDal = userDal;
        _userOftaMappingDal = userOftaMappingDal;
    }

    public UserOftaModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public IUserBuilder Create()
    {
        _aggregate = new UserOftaModel
        {
            ExpiredDate = new DateTime(3000,1,1),
            VerifiedDate = new DateTime(3000,1,1),
            IsVerified = false,
            ListUserMapping = new List<UserOftaMappingModel>(),
        };
        return this;
    }

    public IUserBuilder Load(IUserOftaKey userOftaKey)
    {
        _aggregate = _userDal.GetData(userOftaKey)
            ?? throw new KeyNotFoundException("User Ofta not found");

        _aggregate.ListUserMapping = _userOftaMappingDal.ListData(userOftaKey)?.ToList()
            ?? new List<UserOftaMappingModel>();
        
        return this;
    }

    public IUserBuilder Load(string email)
    {
        _aggregate = _userDal.GetData(email)
                     ?? throw new KeyNotFoundException("Email not found");

        _aggregate.ListUserMapping = _userOftaMappingDal.ListData(_aggregate)?.ToList()
                    ?? new List<UserOftaMappingModel>();

        return this;
    }

    public IUserBuilder UserOftaName(string userName)
    {
        _aggregate.UserOftaName = userName;
        return this;
    }

    public IUserBuilder Email(string email)
    {
        _aggregate.Email = email;
        return this;
    }

    public IUserBuilder AddUserMapping(string userMappingId, string pegId, string userType)
    {
        _aggregate.ListUserMapping.Add(new UserOftaMappingModel(
            _aggregate.UserOftaId,
            userMappingId,
            pegId,
            Enum.Parse<UserTypeEnum>(userType)
        ));

        return this;
    }
}