using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record RemoveTagDocTypeCommand(string DocTypeId, string Tag) : IRequest, IDocTypeKey;

public class RemoveTagDocTypeHandler : IRequestHandler<RemoveTagDocTypeCommand>
{
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public RemoveTagDocTypeHandler(IDocTypeBuilder builder, 
        IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(RemoveTagDocTypeCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty());

        //  BUILDER
        var aggregate = _builder
            .Load(request)
            .RemoveTag(request.Tag)
            .Build();

        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}