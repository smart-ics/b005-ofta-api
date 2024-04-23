using Dawn;
using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record GetPostQuery(string PostId) : IRequest<GetPostResponse>, IPostKey;

public record GetPostResponse(
    string PostId,
    string PostDate,
    string UserOftaId,
    string UserOftaName,
    string Msg,
    int CommentCount,
    int ReactCount,
    IEnumerable<GetPostResponseReact> ListReact,
    IEnumerable<GetPostResponseVisibility> ListVisibility);

public record GetPostResponseReact(
    string PostReactDate,
    string UserOftaKey,
    string UserOftaName);

public record GetPostResponseVisibility(
    string VisibilityReff);

public record GetPostResponseComment(
    string PostReactDate,
    string UserOftaKey,
    string UserOftaName);


public class GetPostQueryHandler : IRequestHandler<GetPostQuery, GetPostResponse>
{
    private readonly IPostBuilder _builder;

    public GetPostQueryHandler(IPostBuilder builder)
    {
        _builder = builder;
    }

    public Task<GetPostResponse> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.PostId, x => x.NotEmpty());

        //  BUILD
        var post = _builder
            .Load(request)
            .Build();

        //  PROJECTION
        var response = new GetPostResponse(
                post.PostId,
                post.PostDate.ToString("yyyy-MM-dd"),
                post.UserOftaId,
                post.UserOftaName,
                post.Msg,
                post.CommentCount,
                post.LikeCount,
                post.ListReact.Select(x => new GetPostResponseReact(
                    x.PostReactDate.ToString("yyyy-MM-dd"),
                    x.UserOftaId,
                    x.UserOftaName
                )).ToList(),
                post.ListVisibility.Select(x => new GetPostResponseVisibility(
                    x.VisibilityReff
                    )).ToList()
        );
        return Task.FromResult(response);
    }
}
