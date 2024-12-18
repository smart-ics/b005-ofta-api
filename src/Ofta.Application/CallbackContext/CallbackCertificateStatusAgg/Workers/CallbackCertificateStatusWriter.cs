using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackCertificateStatusAgg;

namespace Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.Workers;

public interface ICallbackCertificateStatusWriter: INunaWriterWithReturn<CallbackCertificateStatusModel>
{
}

public class CallbackCertificateStatusWriter : ICallbackCertificateStatusWriter
{
    private readonly ICallbackCertificateStatusDal _callbackCertificateStatusDal;

    public CallbackCertificateStatusWriter(ICallbackCertificateStatusDal callbackCertificateStatusDal)
    {
        _callbackCertificateStatusDal = callbackCertificateStatusDal;
    }

    public CallbackCertificateStatusModel Save(CallbackCertificateStatusModel model)
    {
        var callbackCertificateStatusDb = _callbackCertificateStatusDal.GetData(model);
        
        using var trans = TransHelper.NewScope();
        if (callbackCertificateStatusDb is null)
            _callbackCertificateStatusDal.Insert(model);
        else
            _callbackCertificateStatusDal.Update(model);
        trans.Complete();
        
        return model;
    }
}