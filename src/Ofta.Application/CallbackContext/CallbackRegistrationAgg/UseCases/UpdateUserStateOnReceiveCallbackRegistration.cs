using MediatR;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.CallbackContext.CallbackRegistrationAgg.UseCases;

public class UpdateUserStateOnReceiveCallbackRegistration: INotificationHandler<ReceiveCallbackRegistrationEvent>
{
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public UpdateUserStateOnReceiveCallbackRegistration(ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(ReceiveCallbackRegistrationEvent notification, CancellationToken cancellationToken)
    {
        // BUILD
        var agg = _builder
            .Load(notification.Agg)
            .Build();
        
        if (notification.Agg.ManualRegistrationStatus != string.Empty)
            agg = _builder
                .Attach(agg)
                .UserState(TilakaUserState.ManualRegistration)
                .Build();
        
        if (notification.Agg.RegistrationStatus == "S" || notification.Agg.ManualRegistrationStatus == "S")
            agg = _builder
                .Attach(agg)
                .UserState(TilakaUserState.Verified)
                .TilakaName(notification.Agg.TilakaName)
                .Build();
        
        // WRITE
        _ = _writer.Save(agg);
        return Task.CompletedTask;
    }
}