using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Contracts;

public interface IPostVisibilityDal :
    IInsertBulk<PostVisibilityModel>,
    IDelete<IPostKey>,
    IListData<PostVisibilityModel, IPostKey>
{
    
}