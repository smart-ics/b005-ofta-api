namespace Ofta.Domain.DocContext.BundleAgg;

public class BundleDocModel : IBundleKey
{
    public string BundleId { get; set; }
    public int NoUrut { get; set; }
    public string DocId { get; set; }
    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
}
