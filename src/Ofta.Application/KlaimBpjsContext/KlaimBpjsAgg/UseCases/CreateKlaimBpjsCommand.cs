using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record CreateKlaimBpjsCommand(string OrderKlaimBpjsId, string UserOftaId) 
    : IRequest<CreateKlaimBpjsResponse>, IUserOftaKey, IOrderKlaimBpjsKey;

public record CreateKlaimBpjsResponse(string KlaimBpjsId);

public class CreateKlaimBpjsCommandHandler : IRequestHandler<CreateKlaimBpjsCommand, CreateKlaimBpjsResponse>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IOrderKlaimBpjsBuilder _orderKlaimBpjsBuilder;
    private readonly IValidator<CreateKlaimBpjsCommand> _guard;
    private readonly IMediator _mediator;

    public CreateKlaimBpjsCommandHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<CreateKlaimBpjsCommand> guard, 
        IMediator mediator, 
        IOrderKlaimBpjsBuilder orderKlaimBpjsBuilder)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
        _orderKlaimBpjsBuilder = orderKlaimBpjsBuilder;
    }

    public Task<CreateKlaimBpjsResponse> Handle(CreateKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        if (OrderHasBeenIssued(request))
            throw new InvalidOperationException("OrderKlaimBpjs already has KlaimBpjs");
        
        //  BUILD
        var klaimBpjs = _builder
            .Create()
            .OrderKlaimBpjs(request)
            .UserOfta(request)
            .GenListBlueprint()
            .Build();
        
        //  WRITE
        var klaimBpjsModel = _writer.Save(klaimBpjs);
        _mediator.Publish(new CreatedKlaimBpjsEvent(klaimBpjsModel, request), cancellationToken);
        return Task.FromResult(new CreateKlaimBpjsResponse(klaimBpjsModel.KlaimBpjsId));
    }

    private bool OrderHasBeenIssued(IOrderKlaimBpjsKey orderKey)
    {
        var fallbackPolicy = Policy<bool>
            .Handle<KeyNotFoundException>()
            .Fallback(() => true);
        var isExist = fallbackPolicy.Execute(() =>
        {
            _orderKlaimBpjsBuilder.Load(orderKey).Build();
            return false;
        });
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

public record CreatedKlaimBpjsEvent(
    KlaimBpjsModel Aggregate,
    CreateKlaimBpjsCommand Command) : INotification;
