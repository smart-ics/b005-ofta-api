using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public interface  IDocDal :
    IInsert<DocModel>,
    IUpdate<DocModel>,
    IDelete<IDocKey>,
    IGetData<DocModel, IDocKey>,
    IGetData<DocModel, IUploadedDocKey>, 
    IListData<DocModel, Periode, IUserOftaKey>,
    IListData<DocModel, IEnumerable<string>, int>,
    IListData<DocModel, IEnumerable<string>>
{
}