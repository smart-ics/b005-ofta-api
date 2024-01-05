namespace Ofta.Domain.DocContext.DocAgg;

public class DocJurnalModel : IDocKey
{
    public string DocId { get; set; }
    public int NoUrut { get; set; }
    public DateTime JurnalDate { get; set; }
    public string JurnalDesc { get; set; }
    public DocStateEnum DocState { get; set; }
}