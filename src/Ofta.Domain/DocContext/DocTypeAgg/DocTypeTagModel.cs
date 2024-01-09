namespace Ofta.Domain.DocContext.DocTypeAgg;

public class DocTypeTagModel : IDocTypeKey, ITag
{
    public string DocTypeId { get; set; }
    public string Tag { get; set; }
}

public interface ITag
{
    string Tag { get; }
}