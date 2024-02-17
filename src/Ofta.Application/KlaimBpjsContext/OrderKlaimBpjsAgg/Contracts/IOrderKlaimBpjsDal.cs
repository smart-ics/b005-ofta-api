using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Contracts;

public interface IOrderKlaimBpjsDal :
    IInsert<OrderKlaimBpjsModel>,
    IUpdate<OrderKlaimBpjsModel>,
    IDelete<IOrderKlaimBpjsKey>,
    IGetData<OrderKlaimBpjsModel, IOrderKlaimBpjsKey>,
    IListData<OrderKlaimBpjsModel, Periode>
{
}