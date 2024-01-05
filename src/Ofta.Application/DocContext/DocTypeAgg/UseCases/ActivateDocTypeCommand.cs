using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record ActivateDocTypeCommand(string DocTypeId) : IRequest, IDocTypeKey;

public class ActivateDocTypeHandler : IRequestHandler<ActivateDocTypeCommand>
{
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public ActivateDocTypeHandler(IDocTypeBuilder builder, IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(ActivateDocTypeCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty());

        //  BUILDER
        var aggregate = _builder
            .Load(request)
            .IsActive(true)
            .Build();

        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}