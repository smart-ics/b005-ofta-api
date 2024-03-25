namespace Ofta.Domain.UserContext.TeamAgg;

public class TeamModel : ITeamKey
{
    public TeamModel()
    {
    }

    public TeamModel(string id) => TeamId = id;

    public string TeamId { get; set; }
    public string TeamName { get; set; }
    public List<TeamUserOftaModel> ListUserOfta { get; set; }    
}