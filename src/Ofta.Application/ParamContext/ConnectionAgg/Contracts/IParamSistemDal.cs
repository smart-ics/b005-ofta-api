using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.ParamContext.SystemAgg;

namespace Ofta.Application.ParamContext.ConnectionAgg.Contracts;

public interface IParamSistemDal :
    IInsert<ParamSistemModel>,
    IUpdate<ParamSistemModel>,
    IDelete<IParamSistemKey>,
    IGetData<ParamSistemModel, IParamSistemKey>,
    IListData<ParamSistemModel>
{
}