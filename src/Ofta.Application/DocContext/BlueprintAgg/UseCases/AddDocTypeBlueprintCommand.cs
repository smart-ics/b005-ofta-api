﻿using Dawn;
using MediatR;
using Ofta.Application.DocContext.BlueprintAgg.Workers;
using Ofta.Domain.DocContext.BlueprintAgg;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.UseCases;

public record AddDocTypeBlueprintCommand(string BlueprintId, string DocTypeId)
    : IRequest, IBlueprintKey, IDocTypeKey;

public class AddDocTypeBlueprintHandler: IRequestHandler<AddDocTypeBlueprintCommand>
{
    private readonly IBlueprintBuilder _builder;
    private readonly IBlueprintWriter _writer;

    public AddDocTypeBlueprintHandler(IBlueprintBuilder builder, 
        IBlueprintWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(AddDocTypeBlueprintCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.BlueprintId, y => y.NotEmpty())
            .Member(x => x.DocTypeId, y => y.NotEmpty());
        
        //  BUILDER
        var blueprint = _builder
            .Load(request)
            .AddDocType(request)
            .Build();
        
        //  WRITE
        _writer.Save(blueprint);
        return Task.FromResult(Unit.Value);
    }
}
