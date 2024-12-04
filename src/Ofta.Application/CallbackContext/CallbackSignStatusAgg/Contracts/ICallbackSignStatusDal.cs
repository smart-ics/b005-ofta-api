using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;

public interface ICallbackSignStatusDal:
    IInsert<CallbackSignStatusModel>, 
    IUpdate<CallbackSignStatusModel>,
    IGetData<CallbackSignStatusModel, ICallbackSignStatusKey>
{
}