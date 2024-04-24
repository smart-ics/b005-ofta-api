using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsSigneeModel : IKlaimBpjsKey
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsDocTypeId { get; set; }
    public string KlaimBpjsPrintOutId { get; set; }
    public string KlaimBpjsSigneeId { get; set; }
    public int NoUrut { get; set; }
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public string SignTag { get; set; }
    public SignPositionEnum SignPosition { get; set; }
    
}


