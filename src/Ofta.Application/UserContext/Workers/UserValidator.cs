using FluentValidation;
using Ofta.Domain.UserContext;

namespace Ofta.Application.UserContext.Workers;

public class UserValidator : AbstractValidator<UserModel>
{
    public UserValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();    
    }
    
}