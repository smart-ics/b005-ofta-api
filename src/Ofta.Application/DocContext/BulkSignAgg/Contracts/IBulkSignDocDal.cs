using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Contracts;

public interface IBulkSignDocDal:
    IInsertBulk<BulkSignDocModel>,
    IDelete<IBulkSignKey>,
    IListData<BulkSignDocModel, IBulkSignKey>
{
}