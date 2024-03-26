namespace Ofta.Domain.TImelineContext.PostAgg;

public class PostVisibilityModel : IPostKey
{
    public string PostId { get; set; }
    public string VisibilityReff { get; set; }
}