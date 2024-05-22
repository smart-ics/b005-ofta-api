using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;

namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;

public class UpdateWorklistOnCreatedKlaimBpjsEvent : INotificationHandler<CreatedKlaimBpjsEvent>
{
    private readonly IWorkListBpjsBuilder _builder;
    private readonly IWorkListBpjsWriter _writer;

    public UpdateWorklistOnCreatedKlaimBpjsEvent(IWorkListBpjsBuilder builder,
        IWorkListBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(CreatedKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var workListBpjs = _builder
            .Load(notification.Aggregate)
            .KlaimBpjs(notification.Aggregate)
            .Build();

        _ = _writer.Save(workListBpjs);

        return Task.CompletedTask;
    }
}