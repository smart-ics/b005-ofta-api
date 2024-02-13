using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.BlueprintAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.UseCases;

public record RenameBlueprintCommand(string BlueprintId, string NewName) 
    : IRequest, IBlueprintKey;

public class RenameBlueprintHandler : IRequestHandler<RenameBlueprintCommand>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;
    private readonly IValidator<RenameBlueprintCommand> _guard;
    
    public RenameBlueprintHandler(IBlueprintBuilder builder, 
        IBlueprintWriter writer, 
        IValidator<RenameBlueprintCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }
    public Task<Unit> Handle(RenameBlueprintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILDER
        var blueprint = _builder
            .Load(request)
            .Name(request.NewName)
            .Build();
        
        //  WRITE
        _writer.Save(blueprint);
        return Task.FromResult(Unit.Value);
    }
}

public class RenameBlueprintGuard : AbstractValidator<RenameBlueprintCommand>
{
    public RenameBlueprintGuard()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.BlueprintId).NotEmpty();
        RuleFor(x => x.NewName).NotEmpty();
    }
}