using FluentValidation;
using Ofta.Domain.UserContext.TeamAgg;

namespace Ofta.Application.UserContext.TeamAgg.Workers;

public class TeamValidator : AbstractValidator<TeamModel>
{
    public TeamValidator()
    {
        RuleFor(x => x.TeamName).NotEmpty();
    }
}