using MediatR;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record AttachDocPostCommand(string PostId, string DocId) :
    IRequest, IPostKey, IDocKey;

public class AttachDocHandler : IRequestHandler<AttachDocPostCommand>
{
    public Task<Unit> Handle(AttachDocPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}