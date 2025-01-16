using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record DocTypeSetNumberFormatCommand(string DocTypeId, string Format, int ResetBy) : IRequest, IDocTypeKey;
public class DocTypeSetNumberFormatHandler: IRequestHandler<DocTypeSetNumberFormatCommand>
{
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public DocTypeSetNumberFormatHandler(IDocTypeBuilder builder, IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }
    
    public Task<Unit> Handle(DocTypeSetNumberFormatCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty())
            .Member(x => x.Format, y => y.NotEmpty())
            .Member(x => x.ResetBy, y => y.NotNegative());

        // BUILD
        var agg = _builder
            .Load(request)
            .SetDocTypeNumberFormat(request.Format, (ResetByEnum)request.ResetBy)
            .Build();

        // WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}