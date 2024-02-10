using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.DocContext.BundleAgg;

namespace Ofta.Application.DocContext.BundleAgg.Contracts;

public interface IBundleDal :
    IInsert<BundleModel>,
    IUpdate<BundleModel>,
    IDelete<IBundleKey>,
    IGetData<BundleModel, IBundleKey>,
    IListData<BundleModel, Periode>
{
}