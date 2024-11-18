using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Domain.DocContext.BulkSignAgg;

public class BulkSignDocModel: IBulkSignKey, IDocKey
{
    public string BulkSignId { get; set; }
    public string DocId { get; set; }
    public string UploadedDocId { get; set; }
    public RequestBulkSignStateEnum RequestBulkSignState { get; set; }
    public int NoUrut { get; set; }
    public List<BulkSignDocSigneeModel> ListSignee { get; set; }

    public void SyncId() => ListSignee.ForEach(x => x.DocId = DocId);
}

public enum RequestBulkSignStateEnum
{
    Success,
    Failed,
}