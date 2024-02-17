using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsSignModel : IKlaimBpjsKey
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsId2 { get; set; }
    public string KlaimBpjsId3 { get; set; }
    public int NoUrut { get; set; }
    
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public string SignTag { get; set; }
    public SignPositionEnum SignPosition { get; set; }
}