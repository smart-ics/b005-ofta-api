using MediatR;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record ListPostQuery(string UserOftaId, int OffsetNo) :
    IRequest<IEnumerable<ListPostResponse>>, IUserOftaKey;

public record ListPostResponse(
    string PostId,
    string PostDate,
    string UserOftaId,
    string UserOftaName,
    string Msg,
    int CommentCount,
    int ReactCount);

public class ListPostHandler : IRequestHandler<ListPostQuery, IEnumerable<ListPostResponse>>
{
    public Task<IEnumerable<ListPostResponse>> Handle(ListPostQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}