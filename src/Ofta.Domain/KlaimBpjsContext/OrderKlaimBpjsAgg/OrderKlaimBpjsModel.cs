namespace Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

public class OrderKlaimBpjsModel : IOrderKlaimBpjsKey, IRegPasien
{
    public string OrderKlaimBpjsId { get; set; }
    public DateTime OrderKlaimBpjsDate { get; set; }
    public string  UserOftaId { get; set; }
    public string RegId { get; set; }
    public string PasienId { get; set; }
    public string PasienName { get; set; }
    public string NoSep { get; set; }
    public string LayananName { get; set; }
    public string DokterName { get; set; }
    public RajalRanapEnum RajalRanap { get; set; }
}