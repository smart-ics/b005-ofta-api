using MediatR;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.TeamAgg.UseCases;

public record ListTeamQuery()
    : IRequest<IEnumerable<ListTeamResponse>>;

public record ListTeamResponse(
    string TeamId,
    string TeamName
   );


public class ListTeamHandler : IRequestHandler<ListTeamQuery, IEnumerable<ListTeamResponse>>
{

    private readonly ITeamDal _teamDal;
    

    public ListTeamHandler(ITeamDal teamDal)
    {

        _teamDal = teamDal;
    }

    public Task<IEnumerable<ListTeamResponse>> Handle(ListTeamQuery request, CancellationToken cancellationToken)
    {
        

        //  QUERY
        var team = _teamDal.ListData()?.ToList()
            ?? new List<TeamModel>();

        //  RETURN
        var response =
            from c in team
            select new ListTeamResponse
            (
                TeamId: c.TeamId,
                TeamName: c.TeamName
            );


        return Task.FromResult(response);
    }
}