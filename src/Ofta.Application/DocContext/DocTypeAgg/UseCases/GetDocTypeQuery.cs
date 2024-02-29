using Mapster;
using MediatR;
using Ofta.Application.DocContext.DocAgg.UseCases;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record GetDocTypeQuery(string DocTypeId) : IRequest<GetDocTypeResponse>, IDocTypeKey;

public record GetDocTypeResponse(
    string DocTypeId,
    string DocTypeName,
    bool IsStandard,
    bool IsActive,
    string FileUrl,
    IEnumerable<string> ListTag
);

public class GetDocTypeQueryHandler : IRequestHandler<GetDocTypeQuery, GetDocTypeResponse>
{
    private readonly IDocTypeBuilder _builder;

    public GetDocTypeQueryHandler(IDocTypeBuilder builder)
    {
        _builder = builder;
    }

    public Task<GetDocTypeResponse> Handle(GetDocTypeQuery request, CancellationToken cancellationToken)
    {
        var docType = _builder
            .Load(new DocTypeModel(request.DocTypeId))
            .Build();

        var response = new GetDocTypeResponse(
            docType.DocTypeId,
            docType.DocTypeName,
            docType.IsStandard,
            docType.IsActive,
            docType.FileUrl,
            docType.ListTag.Select(x => x.Tag));
        return Task.FromResult(response);
    }
}
