using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.CallbackContext.CallbackRegistrationAgg;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.CallbackContext.CallbackRegistrationAgg.Contracts;

public interface ICallbackRegistrationDal:
    IInsert<CallbackRegistrationModel>, 
    IUpdate<CallbackRegistrationModel>,
    IGetData<CallbackRegistrationModel, ITilakaRegistrationKey>
{
}