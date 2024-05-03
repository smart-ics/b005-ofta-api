
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.UserContext.TeamAgg;

public class TeamUserOftaModel : ITeamKey, IUserOftaKey
{
    public string TeamId { get; set; }
    public string UserOftaId { get; set; }
    public string UserOftaName { get; set; }

}