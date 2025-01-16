using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.Helpers.DocNumberGenerator;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record GetDocNumberPreviewQuery(string DocTypeId): IRequest<GetDocNumberPreviewResponse>, IDocTypeKey;

public record GetDocNumberPreviewResponse(string DocNumber);

public class GetDocNumberPreviewHandler: IRequestHandler<GetDocNumberPreviewQuery, GetDocNumberPreviewResponse>
{
    private readonly IDocTypeBuilder _docTypeBuilder;
    private readonly IDocNumberGenerator _docNumberGenerator;
    private readonly ITglJamDal _tglJamDal;

    public GetDocNumberPreviewHandler(IDocTypeBuilder docTypeBuilder, IDocNumberGenerator docNumberGenerator, ITglJamDal tglJamDal)
    {
        _docTypeBuilder = docTypeBuilder;
        _docNumberGenerator = docNumberGenerator;
        _tglJamDal = tglJamDal;
    }

    public Task<GetDocNumberPreviewResponse> Handle(GetDocNumberPreviewQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.DocTypeId, y => y.NotEmpty());
        
        // QUERY
        var agg = _docTypeBuilder
            .Load(request)
            .Build();

        var docNumberPreview = _docNumberGenerator.FormattingNumber(agg.NumberFormat.Format, 1, _tglJamDal.Now);
        
        // RESPONSE
        return Task.FromResult(new GetDocNumberPreviewResponse(docNumberPreview));
    }
}