using Dawn;
using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record ListPostQuery(string UserOftaId, int PageNo)
    : IRequest<IEnumerable<ListPostResponse>>, IUserOftaKey;

public record ListPostResponse(
    string PostId,
    string PostDate,
    string UserOftaId,
    string UserOftaName,
    string Msg,
    int CommentCount,
    int LikeCount,
    List<GetPostReactResponse> ListReact
    );

public class GetPostReactResponse
{
    public string PostId { get; set; }
    public string PostReactDate { get; set; }
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }
}


public class ListPostHandler : IRequestHandler<ListPostQuery, IEnumerable<ListPostResponse>>
{
    private readonly IPostDal _postDal;
    private readonly IPostReactDal _postReactDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IValidator<ListPostQuery> _guard;

    public ListPostHandler(IPostDal postDal,
        IPostReactDal postReactDal, 
        IValidator<ListPostQuery> guard, 
        IUserOftaDal userOftaDal)
    {
        _postDal = postDal;
        _postReactDal = postReactDal;
        _guard = guard;
        _userOftaDal = userOftaDal;

    }

    public Task<IEnumerable<ListPostResponse>> Handle(ListPostQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        //  QUERY
        var userOfta = _userOftaDal.GetData(request)
                   ?? throw new KeyNotFoundException("User Ofta not found");
        var listPost = _postDal.ListData(userOfta, request.PageNo)?.ToList()
                    ?? new List<PostModel>();
        var listPostReact = _postReactDal.ListData()?.ToList()
                         ?? new List<PostReactModel>();

        //RETURN
        var response =
            from c in listPost
            select new ListPostResponse
            (
                PostId: c.PostId,
                PostDate: $"{c.PostDate:yyyy-MM-dd HH:mm:ss}",
                UserOftaId: c.UserOftaId,
                UserOftaName: c.UserOftaName,
                Msg: c.Msg,
                CommentCount: c.CommentCount,
                LikeCount: c.LikeCount,
                ListReact: listPostReact
                    .Where(x => x.PostId == c.PostId)
                    .Select(x => new GetPostReactResponse
                    {
                        PostId = x.PostId,
                        PostReactDate = x.PostReactDate.ToString("yyyy-MM-dd hh:mm:ss"),
                        UserOftaId = x.UserOftaId,
                        UserOftaName = x.UserOftaName
                    }).ToList()
            );


        return Task.FromResult(response);
    }
}

public class ListPostGuard : AbstractValidator<ListPostQuery>
{
    public ListPostGuard()
    {
        RuleFor(x => x.PageNo).GreaterThan(0);
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}