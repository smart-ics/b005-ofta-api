using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Workers;

public interface IPostBuilder : INunaBuilder<PostModel>
{
    IPostBuilder Create();
    IPostBuilder Load(IPostKey postKey);
    
    IPostBuilder User(IUserOftaKey userOftaKey);
    IPostBuilder Msg(string msg);
    IPostBuilder AddVisibility(string visibilityReff);
    IPostBuilder RemoveVisibility(string visibilityReff);
    IPostBuilder AttachDoc(IDocKey docKey);
}
public class PostBuilder : IPostBuilder
{
    private PostModel _agg = new();
    private readonly ITglJamDal _tglJamDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IPostDal _postDal;
    private readonly IPostVisibilityDal _postVisibilityDal;
    private readonly IPostReactDal _postReactDal;
    private readonly IDocDal _docDal;
    

    public PostBuilder(ITglJamDal tglJamDal, 
        IUserOftaDal userOftaDal, 
        IPostDal postDal, 
        IPostVisibilityDal postVisibilityDal, 
        IPostReactDal postReactDal, 
        IDocDal docDal)
    {
        _tglJamDal = tglJamDal;
        _userOftaDal = userOftaDal;
        _postDal = postDal;
        _postVisibilityDal = postVisibilityDal;
        _postReactDal = postReactDal;
        _docDal = docDal;
    }

    public PostModel Build()
    {
        _agg.RemoveNull();
        return _agg;
    }

    public IPostBuilder Create()
    {
        _agg = new PostModel
        {
            PostDate = _tglJamDal.Now,
            LikeCount = 0,
            ListReact = new List<PostReactModel>(),
            ListVisibility = new List<PostVisibilityModel>()
        };
        return this;
    }

    public IPostBuilder Load(IPostKey postKey)
    {
        _agg = _postDal.GetData(postKey)
               ?? throw new KeyNotFoundException($"Post not found: '{postKey.PostId}'");
        _agg.ListVisibility = _postVisibilityDal.ListData(postKey)?.ToList()
                              ?? new List<PostVisibilityModel>();
        _agg.ListReact = _postReactDal.ListData(postKey)?.ToList()
                         ?? new List<PostReactModel>();
        return this;
    }

    public IPostBuilder User(IUserOftaKey userOftaKey)
    {
        var userOfta = _userOftaDal.GetData(userOftaKey)
            ?? throw new KeyNotFoundException($"User not found: '{userOftaKey.UserOftaId}'");
        _agg.UserOftaId = userOfta.UserOftaId;
        _agg.UserOftaName = userOfta.UserOftaName;
        return this;
    }

    public IPostBuilder Msg(string msg)
    {
        _agg.Msg = msg;
        return this;
    }

    public IPostBuilder AddVisibility(string visibilityReff)
    {
        if (_agg.ListVisibility.Any(x => x.VisibilityReff == visibilityReff))
            return this;
        
        _agg.ListVisibility.Add(new PostVisibilityModel
        {
            VisibilityReff = visibilityReff
        });
        return this;
    }

    public IPostBuilder RemoveVisibility(string visibilityReff)
    {
        _agg.ListVisibility.RemoveAll(x => x.VisibilityReff == visibilityReff);
        return this;
    }

    public IPostBuilder AttachDoc(IDocKey docKey)
    {
        var doc = _docDal.GetData(docKey)
                  ?? throw new KeyNotFoundException($"Document not found: '{docKey.DocId}");
        _agg.DocId = doc.DocId;
        return this;
    }
}