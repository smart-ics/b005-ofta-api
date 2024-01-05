using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record AddTagDocTypeCommand(string DocTypeId, string Tag) : IRequest, IDocTypeKey;

public class AddTagDocTypeHandler : IRequestHandler<AddTagDocTypeCommand>
{
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public AddTagDocTypeHandler(IDocTypeBuilder builder, 
        IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(AddTagDocTypeCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty());

        //  BUILDER
        var aggregate = _builder
            .Load(request)
            .AddTag(request.Tag)
            .Build();

        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}