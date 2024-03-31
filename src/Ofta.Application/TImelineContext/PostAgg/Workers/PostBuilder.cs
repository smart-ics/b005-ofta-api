using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Workers;

public interface IPostBuilder : INunaBuilder<PostModel>
{
    IPostBuilder Create();
    IPostBuilder User(IUserOftaKey userOftaKey);
    IPostBuilder Msg(string msg);
    IPostBuilder AddVisibility(string visibilityReff);
    IPostBuilder RemoveVisibility(string visibilityReff);
}
public class PostBuilder : IPostBuilder
{
    private PostModel _agg = new();
    private readonly ITglJamDal _tglJamDal;
    private readonly IUserOftaDal _userOftaDal;

    public PostBuilder(ITglJamDal tglJamDal, 
        IUserOftaDal userOftaDal)
    {
        _tglJamDal = tglJamDal;
        _userOftaDal = userOftaDal;
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
}