using MediatR;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Application.TImelineContext.CommentAgg.UseCases;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public class IncreaseCommentCounterOnCreatedCommentEventHandler : INotificationHandler<CreatedCommentEvent>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly ICommentDal _commentDal;

    public IncreaseCommentCounterOnCreatedCommentEventHandler(IPostBuilder builder, 
        IPostWriter writer, 
        ICommentDal commentDal)
    {
        _builder = builder;
        _writer = writer;
        _commentDal = commentDal;
    }

    public Task Handle(CreatedCommentEvent notification, CancellationToken cancellationToken)
    {
        IPostKey postKey = new PostModel(notification.Aggregate.PostId);
        var listComment = _commentDal.ListData(postKey)?.ToList() 
                          ?? new List<CommentModel>();
        var agg = _builder
            .Load(new PostModel(notification.Aggregate.PostId))
            .CommentCount(listComment.Count)
            .Build();
        _ = _writer.Save(agg);
        return Task.CompletedTask;
    }
}