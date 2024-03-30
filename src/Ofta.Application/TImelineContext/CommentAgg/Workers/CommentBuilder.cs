using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.TImelineContext.CommentAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.Workers;

public interface ICommentBuilder : INunaBuilder<CommentModel>
{
    
}
public class CommentBuilder : ICommentBuilder
{
    public CommentModel Build()
    {
        throw new NotImplementedException();
    }
}