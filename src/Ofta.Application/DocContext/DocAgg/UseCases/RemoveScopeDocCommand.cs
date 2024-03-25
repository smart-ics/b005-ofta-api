using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record RemoveScopeDocCommand(string DocId, string ScopeReff) : IRequest, IDocKey;

public class RemoveScopeDocHandler : IRequestHandler<RemoveScopeDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IValidator<RemoveScopeDocCommand> _guard;

    public RemoveScopeDocHandler(IDocBuilder builder, IDocWriter writer, IValidator<RemoveScopeDocCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(RemoveScopeDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var teamKey = new TeamModel(request.ScopeReff);
        var userKey = new UserOftaModel(request.ScopeReff);
        var doc = _builder
            .Load(request)
            .RemoveScope<ITeamKey>(teamKey)
            .RemoveScope<IUserOftaKey>(userKey)
            .Build();
        
        //  WRITE
        _writer.Save(doc);
        return Task.FromResult(Unit.Value);
    }
}

public class RemoveScopeDocGuard : AbstractValidator<RemoveScopeDocCommand>
{
    public RemoveScopeDocGuard()
    {
        RuleFor(x => x.DocId).NotEmpty();
        RuleFor(x => x.ScopeReff).NotEmpty();
    }
}