using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;

public interface IBlueprintDocTypeDal :
    IInsertBulk<BlueprintDocTypeModel>,
    IDelete<IBlueprintKey>,
    IListData<BlueprintDocTypeModel, IBlueprintKey>
{
}