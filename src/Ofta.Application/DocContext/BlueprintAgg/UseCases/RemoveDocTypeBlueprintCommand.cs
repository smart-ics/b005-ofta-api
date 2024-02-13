using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.BundleSpecAgg;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.UseCases;

public record RemoveDocTypeBlueprintCommand(string BlueprintId, string DocTypeId)
    : IRequest, IBlueprintKey, IDocTypeKey;

public class RemoveDocTypeBlueprintHandler : IRequestHandler<RemoveDocTypeBlueprintCommand>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;
    private readonly IValidator<RemoveDocTypeBlueprintCommand> _guard;

    public RemoveDocTypeBlueprintHandler(IBlueprintBuilder builder, 
        IBlueprintWriter writer, 
        IValidator<RemoveDocTypeBlueprintCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(RemoveDocTypeBlueprintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILDER
        var blueprint = _builder
            .Load(request)
            .RemoveDocType(request)
            .Build();
        
        //  WRITE
        _writer.Save(blueprint);
        return Task.FromResult(Unit.Value);
    }
}

public class RemoveDocTypeBlueprintGuard : AbstractValidator<RemoveDocTypeBlueprintCommand>
{
    public RemoveDocTypeBlueprintGuard()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.BlueprintId).NotEmpty();
        RuleFor(x => x.DocTypeId).NotEmpty();
    }
}