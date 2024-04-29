using Dawn;
using MediatR;
using Ofta.Application.UserContext.TeamAgg.Workers;
using Ofta.Domain.UserContext.TeamAgg;


namespace Ofta.Application.UserContext.TeamAgg.UseCases;

public record GetTeamQuery(string TeamId) : IRequest<GetTeamResponse>, ITeamKey;

public record GetTeamResponse(
    
      string TeamId,
      string TeamName,
      IEnumerable <GetTeamResponseUserOfta> ListUserOfta

    );

public record GetTeamResponseUserOfta(
    string UserOftaId,
    string UserOftaName
);


public class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, GetTeamResponse>
{
    private readonly ITeamBuilder _builder;

    public GetTeamQueryHandler(ITeamBuilder builder)
    {
        _builder = builder;
    }

    public Task<GetTeamResponse> Handle(GetTeamQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.TeamId, x => x.NotEmpty());

        //  BUILD
        var team = _builder
            .Load(request)
            .Build();

        //  PROJECTION
        var response = new GetTeamResponse(
                team.TeamId,
                team.TeamName, 
                team.ListUserOfta
                    .Select(x => new GetTeamResponseUserOfta(
                        x.UserOftaId,
                        x.UserOftaName
                )).ToList()
                
        );
        return Task.FromResult(response);
    }
}

