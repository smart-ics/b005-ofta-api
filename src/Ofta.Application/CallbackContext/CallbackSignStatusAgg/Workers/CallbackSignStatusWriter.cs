using Nuna.Lib.CleanArchHelper;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Workers;

public interface ICallbackSignStatusWriter: INunaWriterWithReturn<CallbackSignStatusModel>{}

public class CallbackSignStatusWriter: ICallbackSignStatusWriter
{
    private readonly ICallbackSignStatusDal _callbackSignStatusDal;

    public CallbackSignStatusWriter(ICallbackSignStatusDal callbackSignStatusDal)
    {
        _callbackSignStatusDal = callbackSignStatusDal;
    }

    public CallbackSignStatusModel Save(CallbackSignStatusModel model)
    {
        _callbackSignStatusDal.Insert(model);
        return model;
    }
}