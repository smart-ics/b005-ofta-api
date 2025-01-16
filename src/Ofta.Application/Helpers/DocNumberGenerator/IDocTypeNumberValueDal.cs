using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.Helpers.DocNumberGenerator;

public interface IDocTypeNumberValueDal:
    IInsertBulk<DocTypeNumberValueModel>,
    IDelete<IDocTypeKey>, 
    IListData<DocTypeNumberValueModel, IDocTypeKey>
{
}