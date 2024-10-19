using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface ITilakaUserDal:
    IInsert<TilakaUserModel>,
    IUpdate<TilakaUserModel>,
    IGetData<TilakaUserModel, ITilakaRegistrationKey>
{
}