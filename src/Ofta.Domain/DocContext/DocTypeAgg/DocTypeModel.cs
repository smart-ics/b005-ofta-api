namespace Ofta.Domain.DocContext.DocTypeAgg;

public class DocTypeModel : IDocTypeKey
{
    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    public bool IsStandard { get; set; }
    public bool IsActive { get; set; }
    
    public List<DocTypeTagModel> ListTag { get; set; }
}