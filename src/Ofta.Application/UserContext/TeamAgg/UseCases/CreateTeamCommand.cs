using Dawn;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Application.UserContext.TeamAgg.Workers;
using Ofta.Domain.UserContext.TeamAgg;

namespace Ofta.Application.UserContext.TeamAgg.UseCases;

public record CreateTeamCommand(string TeamName) : IRequest<CreateTeamResponse>;

public record CreateTeamResponse(string TeamId);

public class CreateTeamHandler : IRequestHandler<CreateTeamCommand, CreateTeamResponse>
{
    private readonly ITeamBuilder _builder;
    private readonly ITeamWriter _writer;
    private readonly ITeamDal _teamDal;

    public CreateTeamHandler(ITeamBuilder builder, 
        ITeamWriter writer, 
        ITeamDal teamDal)
    {
        _builder = builder;
        _writer = writer;
        _teamDal = teamDal;
    }

    public Task<CreateTeamResponse> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.TeamName, y => y.NotEmpty());
        var listTeam = _teamDal.ListData()?.ToList() ?? new List<TeamModel>();
        if (listTeam.Any(x => string.Equals(x.TeamName, request.TeamName, StringComparison.CurrentCultureIgnoreCase)))
            throw new ArgumentException("Team dengan nama ini sudah pernah dibuat");
        
        //  BUILD
        var agg = _builder
            .Create()
            .Name(request.TeamName)
            .Build();
        
        //  WRITE
        agg = _writer.Save(agg);
        return Task.FromResult(new CreateTeamResponse(agg.TeamId));
    }
}