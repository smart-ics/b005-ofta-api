using MediatR;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record AddReactPostCommand(string PostId, string UserOftaId)
    : IRequest, IPostKey, IUserOftaKey;

public class AddReactPostHandler : IRequestHandler<AddReactPostCommand>
{
    public Task<Unit> Handle(AddReactPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}