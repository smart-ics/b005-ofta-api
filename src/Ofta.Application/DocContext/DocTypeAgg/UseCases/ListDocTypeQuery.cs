using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.UseCases;

public record ListDocTypeQuery(List<string> ListTag) : IRequest<IEnumerable<ListDocTypeResponse>>;

public record ListDocTypeResponse(string DocTypeId, string DocTypeName);

public class listTagRecord: ITag
{
    public listTagRecord(string x) => Tag = x;
    
    public string Tag { get; set; }
}


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

        var listTagId = request.ListTag?
            .Select(x => (ITag)new listTagRecord(x)).ToList() ?? new List<ITag>();
        

        var listTag = _docTypeTagDal.ListData(listTagId)?.ToList() ?? new List<DocTypeTagModel>();
        
        var filteredDocType =(
            from docType in listDocType
            join docTypeTag in listTag on docType.DocTypeId equals docTypeTag.DocTypeId
            select docType).ToList();
        
        var response = filteredDocType.Select(x => new ListDocTypeResponse(x.DocTypeId, x.DocTypeName));
        return Task.FromResult(response);
    }
}