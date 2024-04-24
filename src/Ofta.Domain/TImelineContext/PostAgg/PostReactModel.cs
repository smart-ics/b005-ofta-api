namespace Ofta.Domain.TImelineContext.PostAgg;

public class PostReactModel : IPostKey
{
    public string PostId { get; set; }
    public DateTime PostReactDate { get; set; }
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
}