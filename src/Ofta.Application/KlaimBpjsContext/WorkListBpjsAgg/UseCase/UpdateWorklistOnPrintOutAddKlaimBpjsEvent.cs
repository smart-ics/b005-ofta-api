using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;


namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;


public class UpdateWorklistOnPrintOutAddKlaimBpjsEvent : INotificationHandler<PrintedDocKlaimBpjsEvent>
{
    private readonly IWorkListBpjsBuilder _builder;
    private readonly IWorkListBpjsWriter _writer;

    public UpdateWorklistOnPrintOutAddKlaimBpjsEvent(IWorkListBpjsBuilder builder,
        IWorkListBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(PrintedDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var workListBpjs = _builder
            .Load(notification.Aggregate)
            .WorkState(notification.Aggregate.KlaimBpjsState)
            .Build();

        _ = _writer.Save(workListBpjs);

        return Task.CompletedTask;
    }
}