using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BlueprintAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.Contracts;

public interface IBlueprintDocTypeDal :
    IInsertBulk<BlueprintDocTypeModel>,
    IDelete<IBlueprintKey>,
    IListData<BlueprintDocTypeModel, IBlueprintKey>
{
}