using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;

public interface ICallbackSignStatusDal:
    IInsert<CallbackSignStatusModel>
{
}