using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;


namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record ListPostReactQuery(string PostId)
    : IRequest<IEnumerable<ListPostReactResponse>>, IPostKey;

public record ListPostReactResponse(
    string PostId,
    string PostReactDate,
    string UserOftaId,
    string UserOftaName
    );


public class ListPostReactHandler : IRequestHandler<ListPostReactQuery, IEnumerable<ListPostReactResponse>>
{

    private readonly IPostDal _postDal;
    private readonly IPostReactDal _postReactDal;

    public ListPostReactHandler(IPostDal postDal, IPostReactDal postReactDal)
    {

        _postDal = postDal;
        _postReactDal = postReactDal;
    }

    public Task<IEnumerable<ListPostReactResponse>> Handle(ListPostReactQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        var post= _postDal.GetData(request)
                   ?? throw new KeyNotFoundException("Post not found");

        //  QUERY
        var postCommentReact = _postReactDal.ListData(request)?.ToList()
            ?? new List<PostReactModel>();

        //  RETURN
        var response =
            from c in postCommentReact
            select new ListPostReactResponse
            (
                PostId: c.PostId,
                PostReactDate: $"{c.PostReactDate:yyyy-MM-dd HH:mm:ss}",
                UserOftaId: c.UserOftaId,
                UserOftaName: c.UserOftaName
            );


        return Task.FromResult(response);
    }
}