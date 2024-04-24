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
    int LikeCount
    );


public class ListPostHandler : IRequestHandler<ListPostQuery, IEnumerable<ListPostResponse>>
{
    private readonly IPostDal _postDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IValidator<ListPostQuery> _guard;

    public ListPostHandler(IPostDal postDal,
        IValidator<ListPostQuery> guard, 
        IUserOftaDal userOftaDal)
    {
        _postDal = postDal;
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
                LikeCount: c.LikeCount
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