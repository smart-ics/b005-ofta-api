using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record AddScopeDocCommand(string DocId, string ScopeReff) : IRequest, IDocKey;

public class AddScopeDocHandler : IRequestHandler<AddScopeDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IValidator<AddScopeDocCommand> _guard;
    private readonly ITeamDal _teamDal;
    private readonly IUserOftaDal _userOftaDal;
    public AddScopeDocHandler(IDocBuilder builder, 
        IDocWriter writer, 
        IValidator<AddScopeDocCommand> guard, 
        ITeamDal teamDal, 
        IUserOftaDal userOftaDal)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _teamDal = teamDal;
        _userOftaDal = userOftaDal;
    }

    public Task<Unit> Handle(AddScopeDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var scope = _userOftaDal.GetData(new UserOftaModel(request.ScopeReff)) 
                    ?? (IScope)_teamDal.GetData(new TeamModel(request.ScopeReff));
        if (scope is null)
            throw new KeyNotFoundException("Scope Reff invalid");
        var doc = _builder
            .Load(request)
            .AddScope(scope)
            .Build();

        //  WRITE
        _writer.Save(doc);
        return Task.FromResult(Unit.Value);
    }
}

public class AddScopeDocGuard : AbstractValidator<AddScopeDocCommand>
{
    public AddScopeDocGuard()
    {
        RuleFor(x => x.ScopeReff).NotEmpty();
        RuleFor(x => x.DocId).NotEmpty();
    }
}