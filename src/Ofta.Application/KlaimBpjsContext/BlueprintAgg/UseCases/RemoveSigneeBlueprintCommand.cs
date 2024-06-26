﻿using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.UseCases;

public record RemoveSigneeBlueprintCommand(string BlueprintId, 
    string DocTypeId, string Email)
    : IRequest, IBlueprintKey, IDocTypeKey;

public class RemoveSigneeBlueprintHandler : IRequestHandler<RemoveSigneeBlueprintCommand>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;
    private readonly IValidator<RemoveSigneeBlueprintCommand> _guard;

    public RemoveSigneeBlueprintHandler(IBlueprintBuilder builder,
        IBlueprintWriter writer,
        IValidator<RemoveSigneeBlueprintCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(RemoveSigneeBlueprintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILDER
        var blueprint = _builder
            .Load(request)
            .RemoveSignee(request, request.Email)
            .Build();
        
        //  WRITE
        _writer.Save(blueprint);
        return Task.FromResult(Unit.Value);
    }
}

public class RemoveSigneeBlueprintGuard : AbstractValidator<RemoveSigneeBlueprintCommand>
{
    public RemoveSigneeBlueprintGuard()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.BlueprintId).NotEmpty();
        RuleFor(x => x.DocTypeId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}