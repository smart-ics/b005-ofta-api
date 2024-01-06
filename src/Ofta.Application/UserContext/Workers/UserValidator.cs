using FluentValidation;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Application.UserContext.Workers;

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