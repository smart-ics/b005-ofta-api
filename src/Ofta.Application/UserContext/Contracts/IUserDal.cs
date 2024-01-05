using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext;

namespace Ofta.Application.UserContext.Contracts;

public interface IUserDal :
    IInsert<UserModel>,
    IUpdate<UserModel>,
    IDelete<IUserKey>,
    IGetData<UserModel, IUserKey>
{
}