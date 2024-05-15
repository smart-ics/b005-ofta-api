using MediatR;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;


namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.UseCases.Event;

public class AddWorkListBpjsOnOrderKlaimBpjsCreateOrderEventHandler : INotificationHandler<CreateOrderKlaimBpjsEvent>
{
    private readonly IWorkListBpjsBuilder _workListBpjsBuilder;
    private readonly IWorkListBpjsWriter _workListBpjsWriter;
    

    public AddWorkListBpjsOnOrderKlaimBpjsCreateOrderEventHandler(IWorkListBpjsBuilder workListBpjsBuilder,
        IWorkListBpjsWriter workListBpjsWriter)
    {
        _workListBpjsBuilder = workListBpjsBuilder;
        _workListBpjsWriter = workListBpjsWriter;
    }

    public Task Handle(CreateOrderKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var data = notification.Aggregate;
        var requests = notification.Command;


        var agg = _workListBpjsBuilder
            .Create(notification.Aggregate)
            .Reg(notification.Aggregate)
            .Layanan(notification.Aggregate.LayananName)
            .Dokter(notification.Aggregate.DokterName)
            .RajalRanap(notification.Aggregate.RajalRanap)
            .WorkState(KlaimBpjsStateEnum.Created)
            .Build();

        _workListBpjsWriter.Save(agg);
        return Task.CompletedTask;
    }
}