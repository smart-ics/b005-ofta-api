using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public interface IDocJurnalDal :
    IInsertBulk<DocJurnalModel>,
    IDelete<IDocKey>,
    IListData<DocJurnalModel, IDocKey>
{
}