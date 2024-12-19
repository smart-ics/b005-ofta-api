using System.Text.Json;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.Contracts;
using Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.UseCases;
using Ofta.Application.Helpers;
using Ofta.Domain.CallbackContext.CallbackCertificateStatusAgg;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.Workers;

public interface ICallbackCertificateStatusBuilder : INunaBuilder<CallbackCertificateStatusModel>
{
    ICallbackCertificateStatusBuilder Create(string registerId, string tilakaName);
    ICallbackCertificateStatusBuilder Attach(CallbackCertificateStatusModel model);
    ICallbackCertificateStatusBuilder Load(ITilakaRegistrationKey key);
    ICallbackCertificateStatusBuilder CallbackDate();
    ICallbackCertificateStatusBuilder Status(string certificateStatus);
    ICallbackCertificateStatusBuilder JsonPayload(ReceiveCallbackCertificateStatusCommand payload);
}

public class CallbackCertificateStatusBuilder: ICallbackCertificateStatusBuilder
{
    private CallbackCertificateStatusModel _aggregate;
    private readonly ITglJamDal _tglJamDal;
    private readonly ICallbackCertificateStatusDal _callbackCertificateStatusDal;

    public CallbackCertificateStatusBuilder(ITglJamDal tglJamDal, ICallbackCertificateStatusDal callbackCertificateStatusDal)
    {
        _tglJamDal = tglJamDal;
        _callbackCertificateStatusDal = callbackCertificateStatusDal;
    }

    public CallbackCertificateStatusModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public ICallbackCertificateStatusBuilder Create(string registerId, string tilakaName)
    {
        _aggregate = new CallbackCertificateStatusModel
        {
            RegistrationId = registerId,
            TilakaName = tilakaName,
            CallbackDate = _tglJamDal.Now,
        };

        return this;
    }

    public ICallbackCertificateStatusBuilder Attach(CallbackCertificateStatusModel model)
    {
        _aggregate = model;
        return this;
    }

    public ICallbackCertificateStatusBuilder Load(ITilakaRegistrationKey key)
    {
        _aggregate = _callbackCertificateStatusDal.GetData(key) 
            ?? throw new KeyNotFoundException("CallbackCertificateStatus not found");
        return this;
    }

    public ICallbackCertificateStatusBuilder CallbackDate()
    {
        _aggregate.CallbackDate = _tglJamDal.Now;
        return this;
    }

    public ICallbackCertificateStatusBuilder Status(string certificateStatus)
    {
        _aggregate.CertificateStatus = certificateStatus;

        return this;
    }

    public ICallbackCertificateStatusBuilder JsonPayload(ReceiveCallbackCertificateStatusCommand payload)
    {
        _aggregate.JsonPayload = JsonSerializer.Serialize(payload);
        return this;
    }
}