using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BundleAgg;

namespace Ofta.Application.DocContext.BundleAgg.Contracts;

public interface IBundleDocDal :
    IInsertBulk<BundleDocModel>,
    IDelete<IBundleKey>,
    IListData<BundleDocModel, IBundleKey>
{
}