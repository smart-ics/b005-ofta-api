using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record RemoveSigneeDocCommand(string DocId, string UserOftaId)
    : IRequest, IDocKey, IUserOftaKey;

public class RemoveSigneeDocHandler : IRequestHandler<RemoveSigneeDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;

    public RemoveSigneeDocHandler(IDocBuilder builder, IDocWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(RemoveSigneeDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty())
            .Member(x => x.UserOftaId, y => y.NotEmpty());
        
        //  BUILD
        var aggregate = _builder
            .Load(request)
            .RemoveSignee(request)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}
