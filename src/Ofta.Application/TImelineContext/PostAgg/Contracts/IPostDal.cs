using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Contracts;

public interface IPostDal :
    IInsert<PostModel>,
    IUpdate<PostModel>,
    IDelete<IPostKey>,
    IGetData<PostModel, IPostKey>,
    IListData<PostModel, Periode, IUserOftaKey>
{
}