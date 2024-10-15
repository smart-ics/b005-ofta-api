using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.UserOftaAgg.Contracts;

public interface IUserOftaMappingDal: 
    IInsertBulk<UserOftaMappingModel>,
    IDelete<IUserOftaKey>,
    IListData<UserOftaMappingModel, IUserOftaKey>,
    IListData<UserOftaMappingModel, string>
{
}