using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.Workers;
using Ofta.Domain.CallbackContext.CallbackCertificateStatusAgg;
using Polly;

namespace Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.UseCases;


public class ReceiveCallbackCertificateStatusCommand: IRequest
{
    [JsonPropertyName("user_identifier")]
    public string UserIdentifier { get; set; }
    
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
}

public class ReceiveCallbackCertificateStatusHandler: IRequestHandler<ReceiveCallbackCertificateStatusCommand>
{
    private readonly ICallbackCertificateStatusBuilder _builder;
    private readonly ICallbackCertificateStatusWriter _writer;
    private readonly IMediator _mediator;

    public ReceiveCallbackCertificateStatusHandler(ICallbackCertificateStatusBuilder builder, ICallbackCertificateStatusWriter writer, IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _mediator = mediator;
    }

    public Task<Unit> Handle(ReceiveCallbackCertificateStatusCommand request, CancellationToken cancellationToken)
    {
        // BUILD
        var fallback = Policy<CallbackCertificateStatusModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _builder
                .Create(request.RequestId, request.UserIdentifier, JsonSerializer.Serialize(request))
                .Build());
        
        var agg = fallback.Execute(() =>
            _builder.Load(new CallbackCertificateStatusModel(request.RequestId, request.UserIdentifier)).Build());

        agg = _builder
            .Attach(agg)
            .CallbackDate()
            .Status(request.Status)
            .Build();
        
        // WRITE
        _ = _writer.Save(agg);
        _mediator.Publish(new ReceiveCallbackCertificateStatusEvent(agg, request), CancellationToken.None);
        return Task.FromResult(Unit.Value);
    }
}

public record ReceiveCallbackCertificateStatusEvent(CallbackCertificateStatusModel Agg, ReceiveCallbackCertificateStatusCommand Command): INotification;