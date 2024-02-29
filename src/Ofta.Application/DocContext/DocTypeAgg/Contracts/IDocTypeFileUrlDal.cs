using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Contracts;

public interface IDocTypeFileUrlDal :
    IInsert<IDocTypeFileUrl>,
    IUpdate<IDocTypeFileUrl>,
    IDelete<IDocTypeKey>,
    IGetData<IDocTypeFileUrl, IDocTypeKey>,
    IListData<IDocTypeFileUrl>
{
}