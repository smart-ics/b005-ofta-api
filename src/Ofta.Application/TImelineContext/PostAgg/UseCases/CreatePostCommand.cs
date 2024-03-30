using MediatR;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record CreatePostCommand(string UserOftaId, string Msg)
    : IRequest<CreatePostResponse>, IUserOftaKey;

public record CreatePostResponse(string PostId);


public class CreatePostHandler : IRequestHandler<CreatePostCommand, CreatePostResponse>
{
    public Task<CreatePostResponse> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}