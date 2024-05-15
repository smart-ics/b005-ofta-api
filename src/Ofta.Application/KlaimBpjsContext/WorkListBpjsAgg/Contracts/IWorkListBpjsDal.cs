using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;

public interface IWorkListBpjsDal :
    IInsert<WorkListBpjsModel>,
    IUpdate<WorkListBpjsModel>,
    IDelete<IOrderKlaimBpjsKey>,
    IGetData<WorkListBpjsModel, IOrderKlaimBpjsKey>,
    IListData<WorkListBpjsModel>,
    IListData<WorkListBpjsModel, int>
{
}