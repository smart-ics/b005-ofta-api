using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Domain.DocContext.BulkSignAgg;

public class BulkSignDocModel: IBulkSignKey, IDocKey
{
    public string BulkSignId { get; set; }
    public string DocId { get; set; }
    public string UploadedDocId { get; set; }
    public RequestBulkSignStateEnum RequestBulkSignState { get; set; }
    public int NoUrut { get; set; }
    public string SignTag { get; set; }
    public SignPositionEnum SignPosition { get; set; }
    public string SignPositionDesc { get; set; }
    public string SignUrl { get; set; }
}

public enum RequestBulkSignStateEnum
{
    Success,
    Failed,
}