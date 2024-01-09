using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record ListDocTypeQuery(string Tag) : IRequest<IEnumerable<ListDocTypeResponse>>;

public record ListDocTypeResponse(string DocTypeId, string DocTypeName, string TemplateUrl);

public class ListDocTypeHandler : IRequestHandler<ListDocTypeQuery, IEnumerable<ListDocTypeResponse>>
{
    private readonly IDocTypeDal _docTypeDal;
    private readonly IDocTypeTagDal _docTypeTagDal;

    public ListDocTypeHandler(IDocTypeDal docTypeDal, 
        IDocTypeTagDal docTypeTagDal)
    {
        _docTypeDal = docTypeDal;
        _docTypeTagDal = docTypeTagDal;
    }

    public Task<IEnumerable<ListDocTypeResponse>> Handle(ListDocTypeQuery request, CancellationToken cancellationToken)
    {
        var listDocType = _docTypeDal.ListData()?.ToList() ?? new List<DocTypeModel>();
        ITag tag = new DocTypeTagModel{Tag = request.Tag};
        var listTag = _docTypeTagDal.ListData(tag)?.ToList() ?? new List<DocTypeTagModel>();
        
        var filteredDocType =(
            from docType in listDocType
            join docTypeTag in listTag on docType.DocTypeId equals docTypeTag.DocTypeId
            select docType).ToList();
        
        var response = filteredDocType.Select(x => new ListDocTypeResponse(x.DocTypeId, x.DocTypeName, x.TemplateUrl));
        return Task.FromResult(response);
    }
}