using MediatR;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.TImelineContext.PostAgg;


namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record ListCommentQuery(string PostId)
    : IRequest<IEnumerable<ListCommentResponse>>, IPostKey;

public record ListCommentResponse(
    
    string CommentId, 
    string CommentDate, 
    string PostId,
    string UserOftaId,
    string UserOftaName, 
    string Msg, 
    int ReactCount,
    List<GetCommentReactResponse> ListReact
    );

public class GetCommentReactResponse
{
    public string CommentId { get; set; }
    public string PostReactDate { get; set; }
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
}

public class ListCommentHandler : IRequestHandler<ListCommentQuery, IEnumerable<ListCommentResponse>>
{
    private readonly IPostDal _postDal;
    private readonly ICommentDal _commentDal;
    private readonly ICommentReactDal _commentReactDal;

    public ListCommentHandler(IPostDal postDal, ICommentDal commentDal,ICommentReactDal commentReactDal)
    {
        _postDal = postDal;
        _commentDal = commentDal;
        _commentReactDal = commentReactDal;
    }

    public Task<IEnumerable<ListCommentResponse>> Handle(ListCommentQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        var post = _postDal.GetData(request)
                   ?? throw new KeyNotFoundException("Post not found");

        //  QUERY
        var listComment = _commentDal.ListData(request)?.ToList()
            ?? new List<CommentModel>();
        var listCommentReact = _commentReactDal.ListData()?.ToList()
            ?? new List<CommentReactModel>();

        //  RETURN
        var response =
            from c in listComment
            select new ListCommentResponse
            (
                CommentId: c.CommentId,
                CommentDate: $"{c.CommentDate:yyyy-MM-dd HH:mm:ss}",
                PostId: c.PostId,
                UserOftaId: c.UserOftaId,
                UserOftaName: c.UserOftaName,
                Msg: c.Msg,
                ReactCount: c.ReactCount,
                ListReact: listCommentReact
                    .Where(x => x.CommentId == c.CommentId)
                    .Select(x => new GetCommentReactResponse
                    {
                        CommentId = x.CommentId,
                        PostReactDate = x.CommentReactDate.ToString("yyyy-MM-dd hh:mm:ss"),
                        UserOftaId = x.UserOftaId,
                        UserOftaName = x.UserOftaName
                    }).ToList()
            );


        return Task.FromResult(response);
    }
}