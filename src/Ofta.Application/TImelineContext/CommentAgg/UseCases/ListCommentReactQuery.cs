using MediatR;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;


namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record ListCommentReactQuery(string CommentId)
    : IRequest<IEnumerable<ListCommentReactResponse>>, ICommentKey;

public record ListCommentReactResponse(
    string CommentId,
    string CommentReactDate,
    string UserOftaId,
    string UserOftaName
    );


public class ListCommentReactHandler : IRequestHandler<ListCommentReactQuery, IEnumerable<ListCommentReactResponse>>
{
    
    private readonly ICommentDal _commentDal;
    private readonly ICommentReactDal _commentReactDal;

    public ListCommentReactHandler(ICommentDal commentDal, ICommentReactDal commentReactDal)
    {
        
        _commentDal = commentDal;
        _commentReactDal = commentReactDal;
    }

    public Task<IEnumerable<ListCommentReactResponse>> Handle(ListCommentReactQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        var comment = _commentDal.GetData(request)
                   ?? throw new KeyNotFoundException("Comment not found");

        //  QUERY
        var listCommentReact = _commentReactDal.ListData(request)?.ToList()
            ?? new List<CommentReactModel>();

        //  RETURN
        var response =
            from c in listCommentReact
            select new ListCommentReactResponse
            (
                CommentId: c.CommentId,
                CommentReactDate: $"{c.CommentReactDate:yyyy-MM-dd HH:mm:ss}",
                UserOftaId: c.UserOftaId,
                UserOftaName: c.UserOftaName
            );


        return Task.FromResult(response);
    }
}