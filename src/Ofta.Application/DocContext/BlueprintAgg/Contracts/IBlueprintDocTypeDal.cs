using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BlueprintAgg;
using Ofta.Domain.DocContext.BundleSpecAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.Contracts;

public interface IBlueprintDocTypeDal :
    IInsertBulk<BlueprintDocTypeModel>,
    IDelete<IBlueprintKey>,
    IListData<BlueprintDocTypeModel, IBlueprintKey>
{
}