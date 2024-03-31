using MediatR;
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

public class GetPostQueryHandler : IRequestHandler<GetPostQuery, GetPostResponse>
{
    public Task<GetPostResponse> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
