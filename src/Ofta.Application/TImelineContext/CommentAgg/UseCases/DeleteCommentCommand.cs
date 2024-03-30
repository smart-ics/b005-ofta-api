using MediatR;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record DeleteCommentCommand(string CommentId, string UserOftaId)
    : IRequest, ICommentKey, IUserOftaKey;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand>
{
    public Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}