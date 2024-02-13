using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.BundleSpecAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.UseCases;

public record DeleteBlueprintCommand(string BlueprintId) : IRequest, IBlueprintKey;

public class DeleteBlueprintHandler : IRequestHandler<DeleteBlueprintCommand>
{
    private readonly IBlueprintWriter _writer;
    private readonly IValidator<DeleteBlueprintCommand> _guard;

    public DeleteBlueprintHandler(IBlueprintWriter writer,
        IValidator<DeleteBlueprintCommand> guard)
    {
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(DeleteBlueprintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  WRITE
        _writer.Delete(request);
        return Task.FromResult(Unit.Value);
    }
}

public class DeleteBlueprintGuard : AbstractValidator<DeleteBlueprintCommand>
{
    public DeleteBlueprintGuard()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.BlueprintId).NotEmpty();
    }
}
