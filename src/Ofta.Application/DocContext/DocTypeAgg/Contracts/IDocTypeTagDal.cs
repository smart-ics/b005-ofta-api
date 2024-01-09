using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Contracts;

public interface IDocTypeTagDal :
    IInsertBulk<DocTypeTagModel>,
    IDelete<IDocTypeKey>,
    IListData<DocTypeTagModel, IDocTypeKey>,
    IListData<DocTypeTagModel, ITag>
{
    
}