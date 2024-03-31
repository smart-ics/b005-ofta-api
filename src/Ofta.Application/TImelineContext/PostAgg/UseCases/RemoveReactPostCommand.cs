using MediatR;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record RemoveReactPostCommand(string PostId, string UserOftaId)
    : IRequest, IPostKey, IUserOftaKey;

public class RemoveReachPostHandler : IRequestHandler<RemoveReactPostCommand>
{
    public Task<Unit> Handle(RemoveReactPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}