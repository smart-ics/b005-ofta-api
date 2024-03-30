using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Workers;

public interface IPostBuilder : INunaBuilder<PostModel>
{
}
public class PostBuilder : IPostBuilder
{
    public PostModel Build()
    {
        throw new NotImplementedException();
    }
}