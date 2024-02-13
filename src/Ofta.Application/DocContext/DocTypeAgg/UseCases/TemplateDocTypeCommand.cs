using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record TemplateDocTypeCommand(string DocTypeId, string TemplateUrl)
    : IRequest, IDocTypeKey;

public class TemplateDocTypeHandler : IRequestHandler<TemplateDocTypeCommand>
{
    private DocTypeModel _aggregate = new();
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public TemplateDocTypeHandler(IDocTypeBuilder builder, 
        IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }


    public Task<Unit> Handle(TemplateDocTypeCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty())
            .Member(x => x.TemplateUrl, y => y.NotEmpty());
        
        //  BUILDER
        _aggregate = _builder
            .Load(request)
            .Build();
        
        //  WRITE
        _writer.Save(_aggregate);
        return Task.FromResult(Unit.Value);
    }
}
