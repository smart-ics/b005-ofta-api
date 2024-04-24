using MediatR;
using Ofta.Application.DocContext.DocAgg.UseCases;
using Ofta.Application.TImelineContext.PostAgg.Workers;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public class CreatePostOnSubmittedDocEventHandler : INotificationHandler<SubmittedDocEvent>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;

    public CreatePostOnSubmittedDocEventHandler(IPostBuilder builder, IPostWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(SubmittedDocEvent notification, CancellationToken cancellationToken)
    {
        var agg = _builder
            .Create()
            .User(notification.Aggregate)
            .Msg($"Document created: {notification.Aggregate.RequestedDocUrl}")
            .AttachDoc(notification.Aggregate)
            .Build();
        _ = _writer.Save(agg);
        return Task.CompletedTask;
    }
}