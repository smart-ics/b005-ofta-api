using MediatR;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record UpdateMsgCommentCommand(string CommentId, string Msg, string UserOftaId)
    : IRequest, ICommentKey, IUserOftaKey;

public class UpdateMsgCommentHandler : IRequestHandler<UpdateMsgCommentCommand>
{
    public Task<Unit> Handle(UpdateMsgCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}