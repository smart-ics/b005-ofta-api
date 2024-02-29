namespace Ofta.Domain.UserContext.TeamAgg;

public class TeamModel : ITeamKey
{
    public string TeamId { get; set; }
    public string TeamName { get; set; }
    public List<TeamUserOftaModel> ListUserOfta { get; set; }    
}