using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.UserContext.TilakaAgg;

public class TilakaUserModel: ITilakaRegistrationKey, IUserOftaKey
{
    public TilakaUserModel() { }
    
    public string RegistrationId { get; set; }
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
    public string Email { get; set; }
    public string NomorIdentitas { get; set; }
    public string FotoKtpBase64 { get; set; }
    public string TilakaName { get; set; }
    public DateTime ExpiredDate { get; set; }
    public TilakaUserState UserState { get; set; }
    public TilakaCertificateState CertificateState { get; set; }
    public string RevokeReason { get; set; }
}