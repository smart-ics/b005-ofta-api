using MediatR;
using Ofta.Application.DocContext.BulkSignAgg.Workers;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.UserContext.TilakaAgg.Workers;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public class UpdateSignUrlOnRequestBulkSignSuccess : INotificationHandler<RequestBulkSignSuccessEvent>
{
    private readonly IBulkSignBuilder _bulkSignBuilder;
    private readonly ITilakaUserBuilder _tilakaUserBuilder;
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;

    public UpdateSignUrlOnRequestBulkSignSuccess(IBulkSignBuilder bulkSignBuilder, ITilakaUserBuilder tilakaUserBuilder, IDocBuilder builder, IDocWriter writer)
    {
        _bulkSignBuilder = bulkSignBuilder;
        _tilakaUserBuilder = tilakaUserBuilder;
        _builder = builder;
        _writer = writer;
    }

    public Task Handle(RequestBulkSignSuccessEvent notification, CancellationToken cancellationToken)
    {
        notification.Agg.ListDoc.ForEach(updatedDoc =>
        {
            var bulkSign = _bulkSignBuilder
                .Load(updatedDoc)
                .Build();
            
            var tilakaUser =  _tilakaUserBuilder
                .Load(bulkSign.Email)
                .Build();
            
            var originalDoc = _builder
                .Load(updatedDoc)
                .Build();

            if (originalDoc is null) return;
            
            var originalSignee = originalDoc.ListSignees
                .FirstOrDefault(x => x.UserOftaId == bulkSign.UserOftaId || x.Email == tilakaUser.Email);

            if (originalSignee is not null)
                originalSignee.SignUrl = updatedDoc.SignUrl;
            
            _ = _writer.Save(originalDoc);
        });
        return Task.CompletedTask;
    }
}