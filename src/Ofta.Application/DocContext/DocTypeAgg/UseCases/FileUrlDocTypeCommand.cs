using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record FileUrlDocTypeCommand(string DocTypeId, string FileUrl)
    : IRequest, IDocTypeKey;

public class FileUrlDocTypeHandler : IRequestHandler<FileUrlDocTypeCommand>
{
    private DocTypeModel _aggregate = new();
    private readonly IDocTypeBuilder _builder;
    private readonly IDocTypeWriter _writer;

    public FileUrlDocTypeHandler(IDocTypeBuilder builder, 
        IDocTypeWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }


    public Task<Unit> Handle(FileUrlDocTypeCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty())
            .Member(x => x.FileUrl, y => y.NotEmpty());
        
        //  BUILDER
        _aggregate = _builder
            .Load(request)
            .FileUrl(request.FileUrl)
            .Build();
        
        //  WRITE
        _writer.Save(_aggregate);
        return Task.FromResult(Unit.Value);
    }
}
