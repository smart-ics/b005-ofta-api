using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Contracts;

public interface IPostReactDal :
    IInsertBulk<PostReactModel>,
    IDelete<IPostKey>,
    IListData<PostReactModel, IPostKey>
{
}