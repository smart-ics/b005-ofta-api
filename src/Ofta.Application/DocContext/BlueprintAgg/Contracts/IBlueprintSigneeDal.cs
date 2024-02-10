using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BlueprintAgg;
using Ofta.Domain.DocContext.BundleSpecAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.Contracts;

public interface IBlueprintSigneeDal :
    IInsertBulk<BlueprintSigneeModel>,
    IDelete<IBlueprintKey>,
    IListData<BlueprintSigneeModel, IBlueprintKey>
{
}