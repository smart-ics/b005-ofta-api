using MediatR;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.UseCases;

public record CreateCommentCommand(string Msg, string PostId, string UserOftaId)
    : IRequest<CreateCommentResponse>, IPostKey;

public record CreateCommentResponse(string CommentId);

public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, CreateCommentResponse>
{
    public Task<CreateCommentResponse> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

