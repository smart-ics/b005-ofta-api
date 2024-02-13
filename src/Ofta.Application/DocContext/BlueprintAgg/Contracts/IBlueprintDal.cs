using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BlueprintAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.Contracts;

public interface IBlueprintDal :
    IInsert<BlueprintModel>,
    IUpdate<BlueprintModel>,
    IDelete<IBlueprintKey>,
    IGetData<BlueprintModel, IBlueprintKey>,
    IListData<BlueprintModel>
{
}