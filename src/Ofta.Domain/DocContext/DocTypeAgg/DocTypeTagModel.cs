namespace Ofta.Domain.DocContext.DocTypeAgg;

public class DocTypeTagModel : IDocTypeKey
{
    public string DocTypeId { get; set; }
    public string Tag { get; set; }
}