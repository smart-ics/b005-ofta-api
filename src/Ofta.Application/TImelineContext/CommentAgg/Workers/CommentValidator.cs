using FluentValidation;
using Ofta.Domain.TImelineContext.CommentAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.Workers;

public class CommentValidator : AbstractValidator<CommentModel>
{
    public CommentValidator()
    {
        RuleFor(x => x.Msg).NotEmpty();
        RuleFor(x => x.PostId).NotEmpty();
    }
}