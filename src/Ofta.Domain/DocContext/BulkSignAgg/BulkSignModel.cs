using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.DocContext.BulkSignAgg;

public class BulkSignModel: IBulkSignKey, IUserOftaKey
{
    public string BulkSignId { get; set; }
    public DateTime BulkSignDate { get; set; }
    public string UserOftaId { get; set; }
    public int DocCount { get; set; }
    public BulkSignStateEnum BulkSignState { get; set; }
    public List<BulkSignDocModel> ListDoc { get; set; }

    public void SyncId() => ListDoc.ForEach(x => x.BulkSignId = BulkSignId);
}

public enum BulkSignStateEnum
{
    Requested,
    FailedSign,
    SuccessSign,
}