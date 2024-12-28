using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Domain.CallbackContext.CallbackRegistrationAgg;

public class CallbackRegistrationModel: ITilakaRegistrationKey
{
    public CallbackRegistrationModel() {}
    public CallbackRegistrationModel(string registerId, string tilakaName)
    {
        RegistrationId = registerId;
        TilakaName = tilakaName;
    }
    
    public string RegistrationId { get; set; }
    public string TilakaName { get; set; }
    public string RegistrationStatus { get; set; }
    public string RegistrationReasonCode { get; set; }
    public string ManualRegistrationStatus { get; set; }
    public DateTime CallbackDate { get; set; }
    public string JsonPayload { get; set; }
}