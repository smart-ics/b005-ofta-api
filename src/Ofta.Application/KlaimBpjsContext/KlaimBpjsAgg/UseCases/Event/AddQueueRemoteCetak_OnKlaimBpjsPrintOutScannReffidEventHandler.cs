using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;
using System.Text.Json;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueRemoteCetak_OnKlaimBpjsPrintOutScannReffidEventHandler : INotificationHandler<ScannReffidKlaimBpjsEvent>
{
    private readonly IRemoteCetakBuilder _remoteCetakBuilder;
    private readonly IRemoteCetakWriter _remoteCetakWriter;
    private readonly IAppSettingService _appSettingService;
    private readonly IDocTypeBuilder _docTypeBuilder;

    public AddQueueRemoteCetak_OnKlaimBpjsPrintOutScannReffidEventHandler(IRemoteCetakBuilder remoteCetakBuilder,
        IRemoteCetakWriter remoteCetakWriter,
        IAppSettingService appSettingService,
        IDocTypeBuilder docTypeBuilder)
    {
        _remoteCetakBuilder = remoteCetakBuilder;
        _remoteCetakWriter = remoteCetakWriter;
        _appSettingService = appSettingService;
        _docTypeBuilder = docTypeBuilder;
    }

    public Task Handle(ScannReffidKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var remoteAddr = _appSettingService.RemoteCetakAddress;
        var printOut = notification.Aggregate.ListDocType
            .SelectMany(hdr => hdr.ListPrintOut, (h, d) => new { h.DocTypeId, h.DocTypeName, d.PrintOutReffId });

        foreach (var item in printOut)
        {
            var docTypeModel = new DocTypeModel(item.DocTypeId);
            var docType = _docTypeBuilder.Load(docTypeModel).Build();


            var callbackDataOfta = new
            {
                KlaimBpjsId = notification.Aggregate.KlaimBpjsId,
                PrintOutReffId = item.PrintOutReffId,
                Base64Content = string.Empty
            };
            var agg = _remoteCetakBuilder
                .Create(new RemoteCetakModel(item.PrintOutReffId))
                .RemoteAddr(remoteAddr)
                .JenisDoc(docType.JenisDokRemoteCetak)
                .CallbackDataOfta(JsonSerializer.Serialize(callbackDataOfta))
                .Build();

            _remoteCetakWriter.Save(agg);
        }

        return Task.CompletedTask;
    }
}