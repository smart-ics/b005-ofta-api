using FluentValidation;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Workers;

public class PostValidator : AbstractValidator<PostModel>
{
    public PostValidator()
    {
        RuleFor(x => x.Msg).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}