using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Contracts;

public interface IDocTypeDal :
    IInsert<DocTypeModel>,
    IUpdate<DocTypeModel>,
    IDelete<IDocTypeKey>,
    IGetData<DocTypeModel, IDocTypeKey>,
    IListData<DocTypeModel>
{
    
}