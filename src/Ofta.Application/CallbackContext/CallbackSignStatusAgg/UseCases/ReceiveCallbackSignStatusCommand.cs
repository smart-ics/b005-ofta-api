using System.Text.Json.Serialization;
using MediatR;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Workers;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Domain.UserContext.TilakaAgg;
using Polly;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;

public class ReceiveCallbackSignStatusCommand: IRequest
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }
    
    [JsonPropertyName("user_info")]
    public UserInfoDto UserInfo { get; set; }
    
    [JsonPropertyName("sign_info")]
    public IEnumerable<SignInfoDto> SignInfo { get; set; }
}

public class UserInfoDto
{
    [JsonPropertyName("sequence")]
    public int Sequence { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("user_identifier")]
    public string UserIdentifier { get; set; }
    
    [JsonPropertyName("num_signatures")]
    public int NumSignatures { get; set; }
    
    [JsonPropertyName("num_signatures_done")]
    public int NumSignaturesDone { get; set; }
}

public class SignInfoDto
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("presigned_url")]
    public string PresignedUrl { get; set; }
}

public class ReceiveCallbackSignStatusHandler: IRequestHandler<ReceiveCallbackSignStatusCommand>
{
    private readonly ICallbackSignStatusBuilder _builder;
    private readonly ICallbackSignStatusWriter _writer;
    private readonly IMediator _mediator;

    public ReceiveCallbackSignStatusHandler(ICallbackSignStatusBuilder builder, ICallbackSignStatusWriter writer, IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _mediator = mediator;
    }

    public Task<Unit> Handle(ReceiveCallbackSignStatusCommand request, CancellationToken cancellationToken)
    {
        // BUILD
        var fallback = Policy<CallbackSignStatusModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _builder
                .Create(request.RequestId)
                .UserInfo(new TilakaUserModel(request.UserInfo.UserIdentifier))
                .Build());
        
        var agg = fallback.Execute(() =>
            _builder.Load(new CallbackSignStatusModel(request.RequestId, request.UserInfo.UserIdentifier)).Build());

        request.SignInfo.ForEach(doc =>
        {
            agg = _builder
                .Attach(agg)
                .AddDocument(doc.Filename, doc.PresignedUrl, doc.Status)
                .Build();
        });

        agg = _builder
            .Attach(agg)
            .CallbackDate()
            .Build();
        
        // WRITE
        _ = _writer.Save(agg);
        _mediator.Publish(new ReceiveCallbackSignStatusEvent(agg, request), CancellationToken.None);
        return Task.FromResult(Unit.Value);
    }
}

public record ReceiveCallbackSignStatusEvent(CallbackSignStatusModel Agg, ReceiveCallbackSignStatusCommand Command): INotification;