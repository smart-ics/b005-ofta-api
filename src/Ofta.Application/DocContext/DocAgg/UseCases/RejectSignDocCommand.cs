using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record RejectSignDocCommand(string DocId, string Email) : IRequest,
    IDocKey;

public class RejectSignDocCommandHandler : IRequestHandler<RejectSignDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;

    public RejectSignDocCommandHandler(IDocBuilder builder, IDocWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(RejectSignDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());
        
        //  BUILD
        var aggregate = _builder
            .Load(request)
            .Sign(request.Email)
            .DocState(DocStateEnum.Rejected, request.Email)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}
