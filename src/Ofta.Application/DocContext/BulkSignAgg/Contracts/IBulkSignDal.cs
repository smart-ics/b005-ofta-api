using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Contracts;

public interface IBulkSignDal: 
    IInsert<BulkSignModel>,
    IUpdate<BulkSignModel>,
    IGetData<BulkSignModel, IBulkSignKey>
{
}