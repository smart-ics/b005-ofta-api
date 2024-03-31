using MediatR;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record AddReactCommentCommand(string CommentId, string UserOftaId) 
    : IRequest, ICommentKey, IUserOftaKey;

public class AddReactCommentHandler : IRequestHandler<AddReactCommentCommand>
{
    public Task<Unit> Handle(AddReactCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}