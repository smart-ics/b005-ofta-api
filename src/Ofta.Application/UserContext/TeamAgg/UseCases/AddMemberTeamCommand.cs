using FluentValidation;
using MediatR;
using Ofta.Application.UserContext.TeamAgg.Workers;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.TeamAgg.UseCases;

public record AddMemberTeamCommand(string TeamId, string UserOftaId) : IRequest, ITeamKey, IUserOftaKey;

public class AddMemberTeamHandler : IRequestHandler<AddMemberTeamCommand>
{
    private readonly ITeamBuilder _builder;
    private readonly ITeamWriter _writer;
    private readonly IValidator<AddMemberTeamCommand> _guard;

    public AddMemberTeamHandler(ITeamBuilder builder, 
        ITeamWriter writer, 
        IValidator<AddMemberTeamCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(AddMemberTeamCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var aggregate = _builder
            .Load(request)
            .AddMember(request)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}

public class AddMemberTeamGuard : AbstractValidator<AddMemberTeamCommand>
{
    public AddMemberTeamGuard()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}