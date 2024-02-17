namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public abstract class KlaimBpjsModel : IKlaimBpjsKey
{
    public string KlaimBpjsId { get; set; }
    public DateTime KlaimBpjsDate { get; set; }
    public string Tag { get; set; }
    public KlaimBpjsStateEnum BundleState { get; set; }

    public string RegId { get; set; }
    public string PasienId { get; set; }
    public string PasienName { get; set; }
    public string NoSep { get; set; }

    public string LayananName { get; set; }
    public string DokterName { get; set; }
    public string RajalRanap { get; set; }
    
    
    public List<KlaimBpjsDocModel> ListDoc { get; set; }
}

public enum KlaimBpjsStateEnum
{
    Created = 0,
    Prepared = 1,
    Published = 2
}