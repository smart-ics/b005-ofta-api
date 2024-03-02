using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;

public interface IBlueprintSigneeDal :
    IInsertBulk<BlueprintSigneeModel>,
    IDelete<IBlueprintKey>,
    IListData<BlueprintSigneeModel, IBlueprintKey>
{
}