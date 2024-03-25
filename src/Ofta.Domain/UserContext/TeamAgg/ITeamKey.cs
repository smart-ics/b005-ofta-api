using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.UserContext.TeamAgg;

public interface ITeamKey : IScope
{
    string TeamId { get; }
}