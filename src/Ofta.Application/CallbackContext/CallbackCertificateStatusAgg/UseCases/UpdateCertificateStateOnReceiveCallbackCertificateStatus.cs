using MediatR;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.UseCases;

public class UpdateCertificateStateOnReceiveCallbackCertificateStatus: INotificationHandler<ReceiveCallbackCertificateStatusEvent>
{
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public UpdateCertificateStateOnReceiveCallbackCertificateStatus(ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(ReceiveCallbackCertificateStatusEvent notification, CancellationToken cancellationToken)
    {
        // BUILD
        var agg = _builder
            .Load(notification.Agg)
            .Build();

        switch (notification.Agg.CertificateStatus)
        {
            case "1":
                agg = _builder
                    .Attach(agg)
                    .CertificateState(TilakaCertificateState.VerificationInProgress)
                    .Build();
                break;
            case "2":
                agg = _builder
                    .Attach(agg)
                    .CertificateState(TilakaCertificateState.Registered)
                    .Build();
                break;
            case "3":
                agg = _builder
                    .Attach(agg)
                    .CertificateState(TilakaCertificateState.Active)
                    .Build();
                break;
            case "4":
                agg = _builder
                    .Attach(agg)
                    .CertificateState(TilakaCertificateState.Rejected)
                    .Build();
                break;
        }
        
        // WRITE
        _ = _writer.Save(agg);
        return Task.CompletedTask;
    }
}