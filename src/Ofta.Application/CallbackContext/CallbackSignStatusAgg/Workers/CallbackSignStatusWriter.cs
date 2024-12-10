using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Workers;

public interface ICallbackSignStatusWriter: INunaWriterWithReturn<CallbackSignStatusModel>{}

public class CallbackSignStatusWriter: ICallbackSignStatusWriter
{
    private readonly ICallbackSignStatusDal _callbackSignStatusDal;
    private readonly ICallbackSignStatusDocDal _callbackSignStatusDocDal;

    public CallbackSignStatusWriter(ICallbackSignStatusDal callbackSignStatusDal, ICallbackSignStatusDocDal callbackSignStatusDocDal)
    {
        _callbackSignStatusDal = callbackSignStatusDal;
        _callbackSignStatusDocDal = callbackSignStatusDocDal;
    }

    public CallbackSignStatusModel Save(CallbackSignStatusModel model)
    {
        var callbackSignStatusDb = _callbackSignStatusDal.GetData(model);
        model.SyncId();
        
        using var trans = TransHelper.NewScope();
        if (callbackSignStatusDb is null)
            _callbackSignStatusDal.Insert(model);
        else
            _callbackSignStatusDal.Update(model);
        
        _callbackSignStatusDocDal.Delete(model);
        _callbackSignStatusDocDal.Insert(model.ListDoc);
        
        trans.Complete();
        return model;
    }
}