using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public interface IDocSigneeDal :
    IInsertBulk<DocSigneeModel>,
    IDelete<IDocKey>,
    IListData<DocSigneeModel, IDocKey>
{
}