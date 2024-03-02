using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsModel : IKlaimBpjsKey, IOrderKlaimBpjsKey
{
    public string KlaimBpjsId { get; set; }
    public DateTime KlaimBpjsDate { get; set; }
    public string OrderKlaimBpjsId { get; set; }
    public string UserOftaId { get; set; }
    public KlaimBpjsStateEnum KlaimBpjsState { get; set; }
    public string RegId { get; set; }
    public string PasienId { get; set; }
    public string PasienName { get; set; }
    public string NoSep { get; set; }
    public string LayananName { get; set; }
    public string DokterName { get; set; }
    public RajalRanapEnum RajalRanap { get; set; }
    public List<KlaimBpjsDocModel> ListDoc { get; set; }
    public List<KlaimBpjsJurnalModel> ListJurnal { get; set; }
}

public enum KlaimBpjsStateEnum
{
    Created,    //    pertama kali dibuat
    Listed,     //    list dokumen disesuaikan kebuutuhan
    Sorted,     //    list dokumen sudah disortir/pilah
    Printed,    //    dokumen ter-print semua
    Signing,    //    dokumen sedang di-ttd
    Signed,     //    dokumen sudah ada yg ttd (tapi belum semua)
    Merged,     //    dokumen sudah di-merge
    Downloaded, //    dokumen sudah di-merge  
}