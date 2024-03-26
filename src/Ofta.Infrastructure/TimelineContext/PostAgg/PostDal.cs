using Microsoft.Extensions.Options;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.TimelineContext.PostAgg;

public class PostDal : IPostDal
{
    private readonly DatabaseOptions _opt;

    public PostDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(PostModel model)
    {
        throw new NotImplementedException();
    }

    public void Update(PostModel model)
    {
        throw new NotImplementedException();
    }

    public void Delete(IPostKey key)
    {
        throw new NotImplementedException();
    }

    public PostModel GetData(IPostKey key)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PostModel> ListData(Periode filter1, IUserOftaKey filter2)
    {
        throw new NotImplementedException();
    }
}