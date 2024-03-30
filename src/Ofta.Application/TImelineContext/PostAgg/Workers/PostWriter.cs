using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Workers;

public interface IPostWriter : INunaWriterWithReturn<PostModel>
{
}
public class PostWriter : IPostWriter
{
    public PostModel Save(PostModel model)
    {
        throw new NotImplementedException();
    }
}