﻿using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsCreateCommand(string OrderKlaimBpjsId, string UserOftaId) 
    : IRequest<KlaimBpjsCreateResponse>, IUserOftaKey, IOrderKlaimBpjsKey;


[PublicAPI]
public record KlaimBpjsCreateResponse(string KlaimBpjsId);

public class KlaimBpjsCreateHandler : IRequestHandler<KlaimBpjsCreateCommand, KlaimBpjsCreateResponse>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IOrderKlaimBpjsBuilder _orderKlaimBpjsBuilder;
    private readonly IBlueprintBuilder _blueprintBuilder;
    private readonly IValidator<KlaimBpjsCreateCommand> _guard;
    private readonly IMediator _mediator;

    public KlaimBpjsCreateHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<KlaimBpjsCreateCommand> guard, 
        IOrderKlaimBpjsBuilder orderKlaimBpjsBuilder, 
        IBlueprintBuilder blueprintBuilder, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
        _orderKlaimBpjsBuilder = orderKlaimBpjsBuilder;
        _blueprintBuilder = blueprintBuilder;
    }

    public Task<KlaimBpjsCreateResponse> Handle(KlaimBpjsCreateCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (OrderHasBeenIssued(request, out var klaimBpjsId))
            return Task.FromResult(new KlaimBpjsCreateResponse(klaimBpjsId));

        //  BUILD
        var klaimBpjs = _builder
            .Create()
            .OrderKlaimBpjs(request)
            .UserOfta(request)
            .AddEvent(KlaimBpjsStateEnum.Created, string.Empty)
            .Build();
        
        var bluePrint = _blueprintBuilder.Load(new BlueprintModel("BPX01")).Build();
        bluePrint.ListDocType.ForEach(x =>
        {
            klaimBpjs = _builder
                .Attach(klaimBpjs)
                .AddDocType(x, x.ToBePrinted)
                .Build();
        });
        
        //  WRITE
        var klaimBpjsModel = _writer.Save(klaimBpjs);
        _mediator.Publish(new CreatedKlaimBpjsEvent(klaimBpjsModel, request), cancellationToken);
        return Task.FromResult(new KlaimBpjsCreateResponse(klaimBpjsModel.KlaimBpjsId));
    }

    private bool OrderHasBeenIssued(IOrderKlaimBpjsKey orderKey, out string klaimBpjsId)
    {
        OrderKlaimBpjsModel? orderKlaimBpjs = null;
        var fallbackPolicy = Policy<bool>
            .Handle<KeyNotFoundException>()
            .Fallback(() => true);
        var isExist = fallbackPolicy.Execute(() =>
        {
            orderKlaimBpjs = _orderKlaimBpjsBuilder.Load(orderKey).Build();
            return !orderKlaimBpjs.KlaimBpjsId.IsEmpty();
        });
        klaimBpjsId = orderKlaimBpjs?.KlaimBpjsId??string.Empty;
        return isExist;
    }
}

[UsedImplicitly]
public class CreateKlaimBpjsCommandGuard : AbstractValidator<KlaimBpjsCreateCommand>
{
    public CreateKlaimBpjsCommandGuard()
    {
        RuleFor(x => x.OrderKlaimBpjsId).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}

[PublicAPI]
public record CreatedKlaimBpjsEvent(
    KlaimBpjsModel Aggregate,
    KlaimBpjsCreateCommand Command) : INotification;
