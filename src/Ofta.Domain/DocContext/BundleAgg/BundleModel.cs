namespace Ofta.Domain.DocContext.BundleAgg;

public class BundleModel : IBundleKey
{
    public string BundleId { get; set; }
    public DateTime BundleDate { get; set; }
    public string Tag { get; set; }
    public BundleStateEnum BundleState { get; set; }
    
    public List<BundleDocModel> ListBundleDoc { get; set; }
}

public enum BundleStateEnum
{
    Created = 0,
    Prepared = 1,
    Published = 2
}