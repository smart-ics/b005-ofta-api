using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.TImelineContext.CommentAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.Contracts;

public interface ICommentReactDal :
    IInsertBulk<CommentReactModel>,
    IDelete<ICommentKey>,
    IListData<CommentReactModel, ICommentKey>,
    IListData<CommentReactModel>
{
}