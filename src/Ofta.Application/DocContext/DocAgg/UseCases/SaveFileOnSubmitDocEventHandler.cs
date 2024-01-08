using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public class SaveFileOnSubmitDocEventHandler : INotificationHandler<SubmittedDocEvent>
{
    private readonly ISaveFileService _saveFileService;

    public SaveFileOnSubmitDocEventHandler(ISaveFileService saveFileService)
    {
        _saveFileService = saveFileService;
    }

    public Task Handle(SubmittedDocEvent notification, CancellationToken cancellationToken)
    {
        var saveDocFileRequest = new SaveDocFileRequest
        {
            FilePathName = notification.Aggregate.RequestedDocUrl,
            FileContentBase64 = notification.Command.ContentBase64
        };
        _saveFileService.Execute(saveDocFileRequest);
        return Task.CompletedTask;
    }
}