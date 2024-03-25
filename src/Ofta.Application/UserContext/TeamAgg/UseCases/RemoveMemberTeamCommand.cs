using Dawn;
using FluentValidation;
using MediatR;
using Ofta.Application.UserContext.TeamAgg.Workers;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.TeamAgg.UseCases;

public record RemoveMemberTeamCommand(string TeamId, string UserOftaId)
    : IRequest, ITeamKey, IUserOftaKey;

public class RemoveMemberTeamHandler : IRequestHandler<RemoveMemberTeamCommand>
{
    private readonly ITeamBuilder _teamBuilder;
    private readonly ITeamWriter _teamWriter;

    public RemoveMemberTeamHandler(ITeamBuilder teamBuilder, 
        ITeamWriter teamWriter)
    {
        _teamBuilder = teamBuilder;
        _teamWriter = teamWriter;
    }

    public Task<Unit> Handle(RemoveMemberTeamCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.TeamId, y => y.NotEmpty())
            .Member(x => x.UserOftaId, y => y.NotEmpty());
        
        //  BUILD
        var agg = _teamBuilder
            .Load(request)
            .RemoveMember(request)
            .Build();
        
        //  WRITE
        _teamWriter.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}
