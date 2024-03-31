using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.Contracts;

public interface ICommentDal :
    IInsert<CommentModel>,
    IUpdate<CommentModel>,
    IDelete<ICommentKey>,
    IGetData<CommentModel, ICommentKey>,
    IListData<CommentModel, IPostKey>
{
}