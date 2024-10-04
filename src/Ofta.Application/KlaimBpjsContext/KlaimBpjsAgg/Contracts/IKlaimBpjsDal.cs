using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.RegContext.RegAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsDal :
    IInsert<KlaimBpjsModel>,
    IUpdate<KlaimBpjsModel>,
    IDelete<IKlaimBpjsKey>,
    IGetData<KlaimBpjsModel, IKlaimBpjsKey>,
    IGetData<KlaimBpjsModel, IRegKey>,
    IListData<KlaimBpjsModel, Periode>
{
}