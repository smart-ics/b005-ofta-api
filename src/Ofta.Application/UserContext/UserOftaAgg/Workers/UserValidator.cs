using FluentValidation;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.UserOftaAgg.Workers;

public class UserValidator : AbstractValidator<UserOftaModel>
{
    public UserValidator()
    {
        RuleFor(x => x.UserOftaName)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();    
    }
    
}