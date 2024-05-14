using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.UseCases.Event;

public class AddWorkListBpjsOnOrderKlaimBpjsCreateOrderEventHandler : INotificationHandler<CreateOrderKlaimBpjsEvent>
{
    private readonly IWorkListBpjsBuilder _workListBpjsBuilder;
    private readonly IWorkListBpjsWriter _workListBpjsWriter;
    private readonly IAppSettingService _appSettingService;

    public AddWorkListBpjsOnOrderKlaimBpjsCreateOrderEventHandler(IWorkListBpjsBuilder workListBpjsBuilder,
        IWorkListBpjsWriter workListBpjsWriter,
        IAppSettingService appSettingService)
    {
        _workListBpjsBuilder = workListBpjsBuilder;
        _workListBpjsWriter = workListBpjsWriter;
        _appSettingService = appSettingService;
    }

    public Task Handle(CreateOrderKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {

        var agg = _workListBpjsBuilder
            .Load(new WorkListBpjsModel(notification.Aggregate.OrderKlaimBpjsId))
            .WorkState(KlaimBpjsStateEnum.Created)
            .Build();

        _workListBpjsWriter.Save(agg);
        return Task.CompletedTask;
    }
}