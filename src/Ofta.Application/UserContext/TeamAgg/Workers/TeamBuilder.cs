using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.TeamAgg.Workers;

public interface ITeamBuilder : INunaBuilder<TeamModel>
{
    ITeamBuilder Create();
    ITeamBuilder Load(ITeamKey teamKey);
    ITeamBuilder Attach(TeamModel team);
    ITeamBuilder Name(string name);
    ITeamBuilder AddMember(IUserOftaKey userOftaKey);
    ITeamBuilder RemoveMember(IUserOftaKey userOftaKey);
}
public class TeamBuilder : ITeamBuilder
{
    private readonly ITeamDal _teamDal;
    private readonly ITeamUserOftaDal _teamUserOftaDal;
    private readonly INunaCounterBL _counter;
    private readonly IUserOftaDal _userOftaDal;
    private TeamModel _aggregate = new();

    public TeamBuilder(ITeamDal teamDal, 
        ITeamUserOftaDal teamUserOftaDal, 
        INunaCounterBL counter, 
        IUserOftaDal userOftaDal)
    {
        _teamDal = teamDal;
        _teamUserOftaDal = teamUserOftaDal;
        _counter = counter;
        _userOftaDal = userOftaDal;
    }

    public TeamModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public ITeamBuilder Create()
    {
        _aggregate = new TeamModel
        {
            ListUserOfta = new List<TeamUserOftaModel>()
        };
        return this;
    }

    public ITeamBuilder Load(ITeamKey teamKey)
    {
        _aggregate = _teamDal.GetData(teamKey)
                     ?? throw new KeyNotFoundException("Team not found");
        _aggregate.ListUserOfta = _teamUserOftaDal.ListData(teamKey)?.ToList()
            ?? new List<TeamUserOftaModel>();
        return this;
    }

    public ITeamBuilder Attach(TeamModel team)
    {
        _aggregate = team;
        return this;
    }

    public ITeamBuilder Name(string name)
    {
        _aggregate.TeamName = name;
        return this;
    }

    public ITeamBuilder AddMember(IUserOftaKey userOftaKey)
    {
        if (_aggregate.ListUserOfta.Any(x => x.UserOftaId == userOftaKey.UserOftaId))
            return this;
        
        var userOfta = _userOftaDal.GetData(userOftaKey)
            ?? throw new KeyNotFoundException("User not found");
        _aggregate.ListUserOfta.Add(new TeamUserOftaModel
        {
            UserOftaId = userOfta.UserOftaId
        });
        return this;
    }

    public ITeamBuilder RemoveMember(IUserOftaKey userOftaKey)
    {
        _aggregate.ListUserOfta.RemoveAll(x => x.UserOftaId == userOftaKey.UserOftaId);
        return this;
    }
}