using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public interface  IDocDal :
    IInsert<DocModel>,
    IUpdate<DocModel>,
    IDelete<IDocKey>,
    IGetData<DocModel, IDocKey>,
    IListData<DocModel, Periode, IUserOftaKey>
{
}