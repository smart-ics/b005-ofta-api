using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.DocContext.DraftOrderAgg;
using Usman.Lib.NetStandard.Interfaces;

namespace Ofta.Application.DocContext.DraftOrderAgg;

public interface IDraftOrderDal :
    IInsert<DraftOrderModel>,
    IUpdate<DraftOrderModel>,
    IDelete<IDraftOrderKey>,
    IGetData<DraftOrderModel, IDraftOrderKey>,
    IListData<DraftOrderModel, IUserKey>,
    IListData<DraftOrderModel, Periode>
{
    
}