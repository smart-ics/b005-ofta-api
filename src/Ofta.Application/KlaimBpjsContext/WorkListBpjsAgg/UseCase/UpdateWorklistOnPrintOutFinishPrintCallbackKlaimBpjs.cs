using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;


namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;


public class UpdateWorklistOnPrintOutFinishPrintCallbackKlaimBpjs : INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IWorkListBpjsBuilder _builder;
    private readonly IWorkListBpjsWriter _writer;

    public UpdateWorklistOnPrintOutFinishPrintCallbackKlaimBpjs(IWorkListBpjsBuilder builder,
        IWorkListBpjsWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var workListBpjs = _builder
            .Load(notification.Agg)
            .WorkState(notification.Agg.KlaimBpjsState)
            .Build();

        _ = _writer.Save(workListBpjs);

        return Task.CompletedTask;
    }
}