using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Domain.DocContext.BulkSignAgg;

public class BulkSignDocSigneeModel: IDocKey
{
    public string DocId { get; set; }
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public string SignTag { get; set; }
    public SignPositionEnum SignPosition { get; set; }
    public string SignPositionDesc { get; set; }
    public string SignUrl { get; set; }
}