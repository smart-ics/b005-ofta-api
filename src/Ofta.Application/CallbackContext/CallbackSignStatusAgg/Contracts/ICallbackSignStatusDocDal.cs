using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;

public interface ICallbackSignStatusDocDal:
    IInsertBulk<CallbackSignStatusDocModel>,
    IDelete<ICallbackSignStatusKey>,
    IListData<CallbackSignStatusDocModel, ICallbackSignStatusKey>
{
}