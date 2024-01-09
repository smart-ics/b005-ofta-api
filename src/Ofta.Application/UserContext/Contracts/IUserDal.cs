using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Application.UserContext.Contracts;

public interface IUserOftaDal :
    IInsert<UserOftaModel>,
    IUpdate<UserOftaModel>,
    IDelete<IUserOftaKey>,
    IGetData<UserOftaModel, IUserOftaKey>,
    IListData<UserOftaModel>
{
    UserOftaModel GetData(string email);
}