using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Application.UserContext.Workers;

public interface IUserBuilder : INunaBuilder<UserOftaModel>
{
    IUserBuilder Create();
    IUserBuilder Load(IUserOftaKey userOftaKey);
    IUserBuilder Load(string email);

    IUserBuilder UserOftaName(string userOftaName);
    IUserBuilder Email(string email);
}
public class UserBuilder : IUserBuilder
{
    private UserOftaModel _aggregate = new();
    private readonly IUserOftaDal _userDal;

    public UserBuilder(IUserOftaDal userDal)
    {
        _userDal = userDal;
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
        };
        return this;
    }

    public IUserBuilder Load(IUserOftaKey userOftaKey)
    {
        _aggregate = _userDal.GetData(userOftaKey)
            ?? throw new KeyNotFoundException("User Ofta not found");
        return this;
    }

    public IUserBuilder Load(string email)
    {
        _aggregate = _userDal.GetData(email)
                     ?? throw new KeyNotFoundException("Email not found");
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
}