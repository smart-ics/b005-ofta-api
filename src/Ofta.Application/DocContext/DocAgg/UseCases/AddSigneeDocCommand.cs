﻿using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record AddSigneeDocCommand(string DocId, string UserOftaId, string SignTag, int SignPosition)
    : IRequest, IDocKey, IUserOftaKey;

public class AddSigneeDocHandler : IRequestHandler<AddSigneeDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;

    public AddSigneeDocHandler(IDocBuilder docBuilder, 
        IDocWriter docWriter)
    {
        _builder = docBuilder;
        _writer = docWriter;
    }

    public Task<Unit> Handle(AddSigneeDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty())
            .Member(x => x.UserOftaId, y => y.NotEmpty())
            .Member(x => x.SignTag, y => y.NotEmpty())
            .Member(x => x.SignPosition, y => y.GreaterThan(-1));
        
        //  BUILD
        var signPosition = (SignPositionEnum)request.SignPosition;
        var aggregate = _builder
            .Load(request)
            .AddSignee(request, request.SignTag, signPosition)
            .Build();
        
        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}
