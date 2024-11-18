using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public class UpdateSignUrlOnRequestBulkSignSuccess : INotificationHandler<RequestBulkSignSuccessEvent>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;

    public UpdateSignUrlOnRequestBulkSignSuccess(IDocBuilder builder, IDocWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(RequestBulkSignSuccessEvent notification, CancellationToken cancellationToken)
    {
        notification.Agg.ListDoc.ForEach(doc =>
        {
            var originalDoc = _builder
                .Load(doc)
                .Build();

            if (originalDoc is null) return;
            
            doc.ListSignee.ForEach(updatedSignee =>
            {
                var originalSignee = originalDoc.ListSignees
                    .FirstOrDefault(x => x.UserOftaId == updatedSignee.UserOftaId || x.Email == updatedSignee.Email);

                if (originalSignee is not null)
                    originalSignee.SignUrl = updatedSignee.SignUrl;
            });
            
            _ = _writer.Save(originalDoc);
        });
        return Task.CompletedTask;
    }
}