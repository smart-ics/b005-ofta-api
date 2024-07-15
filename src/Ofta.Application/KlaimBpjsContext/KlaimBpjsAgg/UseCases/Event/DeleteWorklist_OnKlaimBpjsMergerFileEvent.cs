using MediatR;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class DeleteWorklist_OnKlaimBpjsMergerFileEventHandler
    : INotificationHandler<MergerFileBpjsEvent>
{
    private readonly IWorkListBpjsWriter _workListWriter;

    public DeleteWorklist_OnKlaimBpjsMergerFileEventHandler(IWorkListBpjsWriter workListWriter)
    {
        _workListWriter = workListWriter;
    }

    public Task Handle(MergerFileBpjsEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Agg.KlaimBpjsState == KlaimBpjsStateEnum.Completed)
        {
            _workListWriter.Delete(notification.Agg);
        }
        return Task.CompletedTask;
    }

}
