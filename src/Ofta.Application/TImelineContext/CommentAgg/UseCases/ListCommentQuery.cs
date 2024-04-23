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
    int ReactCount
    );


public class ListCommentHandler : IRequestHandler<ListCommentQuery, IEnumerable<ListCommentResponse>>
{
    private readonly IPostDal _postDal;
    private readonly ICommentDal _commentDal;

    public ListCommentHandler(IPostDal postDal, ICommentDal commentDal)
    {
        _postDal = postDal;
        _commentDal = commentDal;
    }

    public Task<IEnumerable<ListCommentResponse>> Handle(ListCommentQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        var post = _postDal.GetData(request)
                   ?? throw new KeyNotFoundException("Post not found");

        //  QUERY
        var listComment = _commentDal.ListData(request)?.ToList()
            ?? new List<CommentModel>();

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
                ReactCount: c.ReactCount
            );


        return Task.FromResult(response);
    }
}