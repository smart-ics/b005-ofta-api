using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;

public class WorkListBpjsModel : IOrderKlaimBpjsKey, IRegPasien, IKlaimBpjsKey
{

    public WorkListBpjsModel(string id) => OrderKlaimBpjsId = id;

    public WorkListBpjsModel()
    {
    }

    public string OrderKlaimBpjsId { get; set; }
    public DateTime OrderKlaimBpjsDate { get; set; }
    public string KlaimBpjsId { get; set; }
    public KlaimBpjsStateEnum WorkState { get; set; }
    public string RegId { get; set; }
    public string PasienId { get; set; }
    public string PasienName { get; set; }
    public string NoSep { get; set; }
    public string LayananName { get; set; }
    public string DokterName { get; set; }
    public RajalRanapEnum RajalRanap { get; set; }
}