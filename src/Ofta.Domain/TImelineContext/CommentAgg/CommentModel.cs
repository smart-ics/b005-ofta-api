namespace Ofta.Domain.TImelineContext.CommentAgg;

public class CommentModel : ICommentKey
{
    public CommentModel(string id) => CommentId = id;

    public CommentModel()
    {
    }
    public string CommentId { get; set; }
    public DateTime CommentDate { get; set; }
    public string PostId { get; set; }
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
    public string Msg { get; set; }
    public int ReactCount { get; set; }
    
    public List<CommentReactModel> ListReact { get; set; }
}