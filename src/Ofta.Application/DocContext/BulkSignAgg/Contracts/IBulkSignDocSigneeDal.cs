using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Contracts;

public interface IBulkSignDocSigneeDal:
    IInsertBulk<BulkSignDocSigneeModel>,
    IDelete<IDocKey>,
    IListData<BulkSignDocSigneeModel, IDocKey>
{
}