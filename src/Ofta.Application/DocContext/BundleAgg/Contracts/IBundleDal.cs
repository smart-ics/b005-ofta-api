using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.DocContext.BundleAgg.Contracts;

public interface IBundleDal :
    IInsert<KlaimBpjsModel>,
    IUpdate<KlaimBpjsModel>,
    IDelete<IKlaimBpjsKey>,
    IGetData<KlaimBpjsModel, IKlaimBpjsKey>,
    IListData<KlaimBpjsModel, Periode>
{
}