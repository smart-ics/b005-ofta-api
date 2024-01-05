using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.UserContext;

namespace Ofta.Application.UserContext.Workers;

public interface IUserBuilder : INunaBuilder<UserModel>
{
    IUserBuilder Create();
    IUserBuilder Load(IUserKey key);

    IUserBuilder UserName(string userName);
    IUserBuilder Email(string email);
}
public class UserBuilder : IUserBuilder
{
    private UserModel _aggregate = new();
    private readonly IUserDal _userDal;
    private readonly ITglJamDal _tglJamDal;

    public UserBuilder(IUserDal userDal, 
        ITglJamDal tglJamDal)
    {
        _userDal = userDal;
        _tglJamDal = tglJamDal;
    }

    public UserModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public IUserBuilder Create()
    {
        _aggregate = new UserModel
        {
            ExpiredDate = new DateTime(3000,1,1),
            VerifiedDate = new DateTime(3000,1,1),
            IsVerified = false,
        };
        return this;
    }

    public IUserBuilder Load(IUserKey key)
    {
        _aggregate = _userDal.GetData(key)
            ?? throw new KeyNotFoundException("User not found");
        return this;
    }

    public IUserBuilder UserName(string userName)
    {
        _aggregate.UserName = userName;
        return this;
    }

    public IUserBuilder Email(string email)
    {
        _aggregate.Email = email;
        return this;
    }
}