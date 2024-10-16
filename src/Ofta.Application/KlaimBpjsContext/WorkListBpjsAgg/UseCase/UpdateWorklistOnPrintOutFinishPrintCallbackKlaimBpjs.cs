using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;
using Polly;


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
        var fallback = Policy<WorkListBpjsModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _builder
                .Create(notification.Agg)
                .Reg((IRegPasien)notification.Agg)
                .Layanan(notification.Agg.LayananName)
                .Dokter(notification.Agg.DokterName)
                .RajalRanap(notification.Agg.RajalRanap)
                .WorkState(KlaimBpjsStateEnum.Completed)
                .Build());
        
        var workListBpjs = fallback.Execute(() =>
            _builder.Load(new WorkListBpjsModel(notification.Agg.OrderKlaimBpjsId)).Build());

        _ = _writer.Save(workListBpjs);

        workListBpjs = _builder
            .Load(notification.Agg)
            .WorkState(notification.Agg.KlaimBpjsState)
            .Build();
        _ = _writer.Save(workListBpjs);

        return Task.CompletedTask;

    }
}