
using Ofta.Domain.DocContext.DocTypeAgg;


namespace Ofta.Domain.DocContext.TagAgg;

public class TagModel : ITag
{
    public string Tag { get; set; }
    public int HitCount { get; set; }
    public int Level { get; set; }
}