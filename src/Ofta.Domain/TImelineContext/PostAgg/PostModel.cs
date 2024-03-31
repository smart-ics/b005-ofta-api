namespace Ofta.Domain.TImelineContext.PostAgg;

public class PostModel : IPostKey
{
    public PostModel()
    {
    }

    public PostModel(string id) => PostId = id;

    public string PostId { get; set; }
    public DateTime PostDate { get; set; }
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
    public string Msg { get; set; }
    public string DocId { get; set; }
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    
    public List<PostReactModel> ListReact { get; set; }
    public List<PostVisibilityModel> ListVisibility { get; set; }
}