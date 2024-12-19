using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Ofta.Application.CallbackContext.CallbackRegistrationAgg.Workers;
using Ofta.Domain.CallbackContext.CallbackRegistrationAgg;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Polly;

namespace Ofta.Application.CallbackContext.CallbackRegistrationAgg.UseCases;

public class ReceiveCallbackRegistrationCommand: IRequest
{
    [JsonPropertyName("registerId")]
    public string RegisterId { get; set; }
    
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("data")]
    public RegistrationDataDto Data { get; set; }
}

public class RegistrationDataDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("nik")]
    public bool? Nik { get; set; }
    
    [JsonPropertyName("nama")]
    public bool? Nama { get; set; }
    
    [JsonPropertyName("photo_selfie")]
    public string PhotoSelfie { get; set; }
    
    [JsonPropertyName("fr_score")]
    public string? FrScore { get; set; }
    
    [JsonPropertyName("fr_score_percentage")]
    public string? FrScorePercentage { get; set; }
    
    [JsonPropertyName("liveness_result")]
    public bool LivenessResult { get; set; }
    
    [JsonPropertyName("liveness_fail_message")]
    public string LivenessFailMessage { get; set; }
    
    [JsonPropertyName("summary_verification_result")]
    public bool SummaryVerificationResult { get; set; }
    
    [JsonPropertyName("tilaka_name")]
    public string? TilakaName { get; set; }
    
    [JsonPropertyName("reason_code")]
    public string ReasonCode { get; set; }
    
    [JsonPropertyName("date_of_birth")]
    public bool? DateOfBirth { get; set; }
    
    [JsonPropertyName("manual_registration_status")]
    public string? ManualRegistrationStatus { get; set; }
}

public class ReceiveCallbackRegistrationHandler: IRequestHandler<ReceiveCallbackRegistrationCommand>
{
    private readonly ICallbackRegistrationBuilder _builder;
    private readonly ICallbackRegistrationWriter _writer;
    private readonly IMediator _mediator;

    public ReceiveCallbackRegistrationHandler(ICallbackRegistrationBuilder builder, ICallbackRegistrationWriter writer, IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _mediator = mediator;
    }

    public Task<Unit> Handle(ReceiveCallbackRegistrationCommand request, CancellationToken cancellationToken)
    {
        // BUILD
        var fallback = Policy<CallbackRegistrationModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _builder
                .Create(request.RegisterId)
                .Build());
        
        var agg = fallback.Execute(() =>
            _builder.Load(new CallbackRegistrationModel(request.RegisterId, request.Data.TilakaName ?? string.Empty)).Build());

        agg = _builder
            .Attach(agg)
            .CallbackDate()
            .TilakaName(request.Data.TilakaName ?? string.Empty)
            .Status(request.Data.Status, request.Data.ReasonCode, request.Data.ManualRegistrationStatus ?? string.Empty)
            .JsonPayload(request)
            .Build();
        
        // WRITE
        _ = _writer.Save(agg);
        _mediator.Publish(new ReceiveCallbackRegistrationEvent(agg, request), CancellationToken.None);
        return Task.FromResult(Unit.Value);
    }
}

public record ReceiveCallbackRegistrationEvent(CallbackRegistrationModel Agg, ReceiveCallbackRegistrationCommand Command): INotification;