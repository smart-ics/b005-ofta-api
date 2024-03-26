namespace Ofta.Domain.TImelineContext.CommentAgg;

public class CommentReactModel : ICommentKey
{
    public string CommentId { get; set; }
    public DateTime CommentReactDate { get; set; }
    public string UserOftaId { get; set; }
}