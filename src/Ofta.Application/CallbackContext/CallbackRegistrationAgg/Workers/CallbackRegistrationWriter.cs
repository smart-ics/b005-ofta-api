using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.CallbackContext.CallbackRegistrationAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackRegistrationAgg;

namespace Ofta.Application.CallbackContext.CallbackRegistrationAgg.Workers;

public interface ICallbackRegistrationWriter: INunaWriterWithReturn<CallbackRegistrationModel>
{
}

public class CallbackRegistrationWriter: ICallbackRegistrationWriter
{
    private readonly ICallbackRegistrationDal _callbackRegistrationDal;

    public CallbackRegistrationWriter(ICallbackRegistrationDal callbackRegistrationDal)
    {
        _callbackRegistrationDal = callbackRegistrationDal;
    }

    public CallbackRegistrationModel Save(CallbackRegistrationModel model)
    {
        var callbackRegistrationDb = _callbackRegistrationDal.GetData(model);
        
        using var trans = TransHelper.NewScope();
        if (callbackRegistrationDb is null)
            _callbackRegistrationDal.Insert(model);
        else
            _callbackRegistrationDal.Update(model);
        trans.Complete();
        
        return model;
    }
}