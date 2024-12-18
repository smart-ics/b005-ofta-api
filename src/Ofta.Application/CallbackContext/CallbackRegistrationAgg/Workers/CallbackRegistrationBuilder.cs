using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.CallbackContext.CallbackRegistrationAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Domain.CallbackContext.CallbackRegistrationAgg;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.CallbackContext.CallbackRegistrationAgg.Workers;


public interface ICallbackRegistrationBuilder : INunaBuilder<CallbackRegistrationModel>
{
    ICallbackRegistrationBuilder Create(string registerId, string tilakaName, string jsonPayload);
    ICallbackRegistrationBuilder Attach(CallbackRegistrationModel model);
    ICallbackRegistrationBuilder Load(ITilakaRegistrationKey key);
    ICallbackRegistrationBuilder CallbackDate();
    ICallbackRegistrationBuilder Status(string registrationStatus, string reasonCode, string manualRegistrationStatus);
}

public class CallbackRegistrationBuilder: ICallbackRegistrationBuilder
{
    private CallbackRegistrationModel _aggregate;
    private readonly ITglJamDal _tglJamDal;
    private readonly ICallbackRegistrationDal _callbackRegistrationDal;

    public CallbackRegistrationBuilder(ITglJamDal tglJamDal, ICallbackRegistrationDal callbackRegistrationDal)
    {
        _tglJamDal = tglJamDal;
        _callbackRegistrationDal = callbackRegistrationDal;
    }

    public CallbackRegistrationModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public ICallbackRegistrationBuilder Create(string registerId, string tilakaName, string jsonPayload)
    {
        _aggregate = new CallbackRegistrationModel
        {
            RegistrationId = registerId,
            TilakaName = tilakaName,
            CallbackDate = _tglJamDal.Now,
            JsonPayload = jsonPayload,
        };

        return this;
    }

    public ICallbackRegistrationBuilder Attach(CallbackRegistrationModel model)
    {
        _aggregate = model;
        return this;
    }

    public ICallbackRegistrationBuilder Load(ITilakaRegistrationKey key)
    {
        _aggregate = _callbackRegistrationDal.GetData(key) 
            ?? throw new KeyNotFoundException("CallbackRegistration not found");
        return this;
    }

    public ICallbackRegistrationBuilder CallbackDate()
    {
        _aggregate.CallbackDate = _tglJamDal.Now;
        return this;
    }

    public ICallbackRegistrationBuilder Status(string registrationStatus, string reasonCode, string manualRegistrationStatus)
    {
        _aggregate.RegistrationStatus = registrationStatus;
        _aggregate.RegistrationReasonCode = reasonCode;
        _aggregate.ManualRegistrationStatus = manualRegistrationStatus;

        return this;
    }
}