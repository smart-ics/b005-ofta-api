using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;

public interface IBlueprintDal :
    IInsert<BlueprintModel>,
    IUpdate<BlueprintModel>,
    IDelete<IBlueprintKey>,
    IGetData<BlueprintModel, IBlueprintKey>,
    IListData<BlueprintModel>
{
}