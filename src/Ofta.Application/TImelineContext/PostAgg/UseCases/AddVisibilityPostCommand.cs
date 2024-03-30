using MediatR;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record AddVisibilityPostCommand(string PostId, string VisibilityReff)
    : IRequest, IPostKey;

public record AddVisibilityPostHandler : IRequestHandler<AddVisibilityPostCommand>
{
    public Task<Unit> Handle(AddVisibilityPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}