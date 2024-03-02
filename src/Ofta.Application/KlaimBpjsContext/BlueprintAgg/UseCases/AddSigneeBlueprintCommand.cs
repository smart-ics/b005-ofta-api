using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.UseCases;

public record AddSigneeBlueprintCommand(string BlueprintId, 
    string DocTypeId, string Email, string SignTag, 
    int SignPosition) : IRequest, IBlueprintKey, IDocTypeKey;

public class AddSigneeBlueprintHandler : IRequestHandler<AddSigneeBlueprintCommand>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;
    private readonly IValidator<AddSigneeBlueprintCommand> _guard;

    public AddSigneeBlueprintHandler(IBlueprintBuilder builder, 
        IBlueprintWriter writer, IValidator<AddSigneeBlueprintCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(AddSigneeBlueprintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILDER
        var blueprint = _builder
            .Load(request)
            .AddSignee(request, request.Email, request.SignTag, (SignPositionEnum)request.SignPosition)
            .Build();
        
        //  WRITE
        _writer.Save(blueprint);
        return Task.FromResult(Unit.Value);
    }
}

public class AddSignBlueprintGuard : AbstractValidator<AddSigneeBlueprintCommand>
{
    public AddSignBlueprintGuard()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.BlueprintId).NotEmpty();
        RuleFor(x => x.DocTypeId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.SignPosition)
            .Must(x => Enum.IsDefined(typeof(SignPositionEnum), x));
    }
}