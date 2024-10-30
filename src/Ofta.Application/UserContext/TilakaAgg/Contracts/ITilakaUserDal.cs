using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface ITilakaUserDal:
    IInsert<TilakaUserModel>,
    IUpdate<TilakaUserModel>,
    IGetData<TilakaUserModel, IUserOftaKey>,
    IGetData<TilakaUserModel, ITilakaRegistrationKey>,
    IGetData<TilakaUserModel, string>
{
}