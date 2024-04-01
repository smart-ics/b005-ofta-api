using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.Workers;

public interface ICommentBuilder : INunaBuilder<CommentModel>
{
    ICommentBuilder Create();
    ICommentBuilder Load(ICommentKey commentKey);
    ICommentBuilder Attach(CommentModel comment);
    ICommentBuilder Post(IPostKey postKey);
    ICommentBuilder Msg(string msg);
    ICommentBuilder User(IUserOftaKey userOftaKey);

}
public class CommentBuilder : ICommentBuilder
{
    private CommentModel _agg = new();
    private readonly ITglJamDal _tglJamDal;
    private readonly IPostDal _postDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly ICommentDal _commentDal;
    private readonly ICommentReactDal _commentReactDal;

    public CommentBuilder(ITglJamDal tglJamDal, 
        IPostDal postDal, 
        IUserOftaDal userOftaDal, 
        ICommentDal commentDal, 
        ICommentReactDal commentReactDal)
    {
        _tglJamDal = tglJamDal;
        _postDal = postDal;
        _userOftaDal = userOftaDal;
        _commentDal = commentDal;
        _commentReactDal = commentReactDal;
    }

    public CommentModel Build()
    {
        _agg.RemoveNull();
        return _agg;
    }

    public ICommentBuilder Create()
    {
        _agg = new CommentModel
        {
            CommentDate = _tglJamDal.Now,
            ListReact = new List<CommentReactModel>()
        };
        return this;
    }

    public ICommentBuilder Load(ICommentKey commentKey)
    {
        _agg = _commentDal.GetData(commentKey)
               ?? throw new KeyNotFoundException($"Comment not found: '{commentKey.CommentId}'");
        _agg.ListReact = _commentReactDal.ListData(commentKey)?.ToList()
                         ?? new List<CommentReactModel>();
        return this;
    }

    public ICommentBuilder Attach(CommentModel comment)
    {
        _agg = comment;
        return this;
    }

    public ICommentBuilder Post(IPostKey postKey)
    {
        var post = _postDal.GetData(postKey)
            ?? throw new KeyNotFoundException($"Post not found: '{postKey.PostId}");
        _agg.PostId = post.PostId;
        return this;
    }

    public ICommentBuilder Msg(string msg)
    {
        _agg.Msg = msg;
        return this;
    }

    public ICommentBuilder User(IUserOftaKey userOftaKey)
    {
        var userOfta = _userOftaDal.GetData(userOftaKey)
                       ?? throw new KeyNotFoundException($"User not found: '{userOftaKey.UserOftaId}");

        _agg.UserOftaId = userOfta.UserOftaId;
        return this;
    }
}