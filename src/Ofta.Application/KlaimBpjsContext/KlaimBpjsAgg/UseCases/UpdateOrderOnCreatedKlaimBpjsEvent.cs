using MediatR;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public class UpdateOrderOnCreatedKlaimBpjsEvent : INotificationHandler<CreatedKlaimBpjsEvent>
{
    private readonly IOrderKlaimBpjsBuilder _builder;
    private readonly IOrderKlaimBpjsWriter _writer;

    public UpdateOrderOnCreatedKlaimBpjsEvent(IOrderKlaimBpjsBuilder builder, 
        IOrderKlaimBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(CreatedKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var orderKlaimBpjs = _builder
            .Load(notification.Aggregate)
            .KlaimBpjs(notification.Aggregate)
            .Build();
        
        _ = _writer.Save(orderKlaimBpjs);
        
        return Task.CompletedTask;
    }
}