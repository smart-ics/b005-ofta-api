using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Domain.CallbackContext.CallbackCertificateStatusAgg;

public class CallbackCertificateStatusModel: ITilakaRegistrationKey
{
    public CallbackCertificateStatusModel() {}
    public CallbackCertificateStatusModel(string registerId, string tilakaName)
    {
        RegistrationId = registerId;
        TilakaName = tilakaName;
    }
    
    public string RegistrationId { get; set; }
    public string TilakaName { get; set; }
    public string CertificateStatus { get; set; }
    public DateTime CallbackDate { get; set; }
    public string JsonPayload { get; set; }
}