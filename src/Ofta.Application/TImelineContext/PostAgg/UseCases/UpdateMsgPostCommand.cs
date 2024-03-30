using MediatR;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record UpdateMsgPostCommand(string PostId, string Msg)
    : IRequest, IPostKey;

public class UpdateMsgPostHandler : IRequestHandler<UpdateMsgPostCommand>
{
    public Task<Unit> Handle(UpdateMsgPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}