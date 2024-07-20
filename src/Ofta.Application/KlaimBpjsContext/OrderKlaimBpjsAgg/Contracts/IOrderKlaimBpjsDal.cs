using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.RegContext.RegAgg;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Contracts;

public interface IOrderKlaimBpjsDal :
    IInsert<OrderKlaimBpjsModel>,
    IUpdate<OrderKlaimBpjsModel>,
    IDelete<IOrderKlaimBpjsKey>,
    IGetData<OrderKlaimBpjsModel, IOrderKlaimBpjsKey>,
    IGetData<OrderKlaimBpjsModel, IRegKey>,
    IListData<OrderKlaimBpjsModel, Periode>
{
}