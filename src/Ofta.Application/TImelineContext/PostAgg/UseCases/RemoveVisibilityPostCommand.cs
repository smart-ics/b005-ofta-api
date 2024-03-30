using MediatR;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record RemoveVisibilityPostCommand(string PostId, string VisibilityReff)
    : IRequest, IPostKey;

public class RemoveVisibilityPostHandler : IRequestHandler<RemoveVisibilityPostCommand>
{
    public Task<Unit> Handle(RemoveVisibilityPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}