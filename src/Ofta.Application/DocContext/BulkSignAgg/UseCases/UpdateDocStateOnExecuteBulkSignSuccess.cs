using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public class UpdateDocStateOnExecuteBulkSignSuccess: INotificationHandler<ExecuteBulkSignSuccessEvent>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;

    public UpdateDocStateOnExecuteBulkSignSuccess(IDocBuilder builder, IDocWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(ExecuteBulkSignSuccessEvent notification, CancellationToken cancellationToken)
    {
        notification.Agg.ListDoc.ForEach(doc =>
        {
            var originalDoc = _builder
                .Load(doc)
                .Sign(notification.Command.Email)
                .AddJurnal(DocStateEnum.Signed, notification.Command.Email)
                .Build();
            
            if (originalDoc.ListSignees.Count > 1)
            {
                var index = originalDoc.ListSignees.FindIndex(x => x.Email == notification.Command.Email);
                if (index >= 0 && index != originalDoc.ListSignees.Count - 1)
                    originalDoc.ListSignees.ElementAt(index + 1).IsHidden = false;
            }
            
            _ = _writer.Save(originalDoc);
        });
        return Task.CompletedTask;
    }
}