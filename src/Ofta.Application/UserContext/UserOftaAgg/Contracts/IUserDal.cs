using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.UserOftaAgg.Contracts;

public interface IUserOftaDal :
    IInsert<UserOftaModel>,
    IUpdate<UserOftaModel>,
    IDelete<IUserOftaKey>,
    IGetData<UserOftaModel, IUserOftaKey>,
    IListData<UserOftaModel>
{
    UserOftaModel GetData(string email);
}