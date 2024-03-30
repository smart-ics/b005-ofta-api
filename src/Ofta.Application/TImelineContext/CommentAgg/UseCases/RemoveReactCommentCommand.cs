using MediatR;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record RemoveReactCommentCommand(string CommentId, string UserOftaId)
    : IRequest, ICommentKey, IUserOftaKey;

public class RemvoeReactCommentHandler : IRequestHandler<RemoveReactCommentCommand>
{
    public Task<Unit> Handle(RemoveReactCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}