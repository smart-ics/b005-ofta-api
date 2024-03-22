using FluentValidation;
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

public record CreateKlaimBpjsCommand(string OrderKlaimBpjsId, string UserOftaId) 
    : IRequest<CreateKlaimBpjsResponse>, IUserOftaKey, IOrderKlaimBpjsKey;


[PublicAPI]
public record CreateKlaimBpjsResponse(string KlaimBpjsId);

public class CreateKlaimBpjsCommandHandler : IRequestHandler<CreateKlaimBpjsCommand, CreateKlaimBpjsResponse>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IOrderKlaimBpjsBuilder _orderKlaimBpjsBuilder;
    private readonly IBlueprintBuilder _blueprintBuilder;
    private readonly IValidator<CreateKlaimBpjsCommand> _guard;
    private readonly IMediator _mediator;

    public CreateKlaimBpjsCommandHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<CreateKlaimBpjsCommand> guard, 
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

    public Task<CreateKlaimBpjsResponse> Handle(CreateKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        if (OrderHasBeenIssued(request, out var klaimBpjsId))
            throw new InvalidOperationException($"Order has been issued as Klaim {klaimBpjsId})");
        
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
                .AddDocType(x)
                .Build();
        });
        
        
        //  WRITE
        var klaimBpjsModel = _writer.Save(klaimBpjs);
        _mediator.Publish(new CreatedKlaimBpjsEvent(klaimBpjsModel, request), cancellationToken);
        return Task.FromResult(new CreateKlaimBpjsResponse(klaimBpjsModel.KlaimBpjsId));
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

public class CreateKlaimBpjsCommandGuard : AbstractValidator<CreateKlaimBpjsCommand>
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
    CreateKlaimBpjsCommand Command) : INotification;
