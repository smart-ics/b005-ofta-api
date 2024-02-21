using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public abstract class KlaimBpjsModel : IKlaimBpjsKey
{
    public string KlaimBpjsId { get; set; }
    public DateTime KlaimBpjsDate { get; set; }
    public string UserOftaId { get; set; }
    public KlaimBpjsStateEnum BundleState { get; set; }
    public string RegId { get; set; }
    public string PasienId { get; set; }
    public string PasienName { get; set; }
    public string NoSep { get; set; }
    public string LayananName { get; set; }
    public string DokterName { get; set; }
    public RajalRanapEnum RajalRanap { get; set; }
    public List<KlaimBpjsDocModel> ListDoc { get; set; }
}

public enum KlaimBpjsStateEnum
{
    Created = 0,
    Prepared = 1,
    Published = 2
}