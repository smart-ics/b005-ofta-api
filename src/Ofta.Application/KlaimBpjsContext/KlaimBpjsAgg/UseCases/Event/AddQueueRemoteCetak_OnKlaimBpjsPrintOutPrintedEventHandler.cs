using System.Text.Json;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueRemoteCetakOnKlaimBpjsPrintOutPrintedEventHandler : INotificationHandler<PrintedDocKlaimBpjsEvent>
{
    private readonly IRemoteCetakBuilder _remoteCetakBuilder;
    private readonly IRemoteCetakWriter _remoteCetakWriter;
    private readonly IAppSettingService _appSettingService;
    private readonly IDocTypeBuilder _docTypeBuilder;

    public AddQueueRemoteCetakOnKlaimBpjsPrintOutPrintedEventHandler(IRemoteCetakBuilder remoteCetakBuilder, 
        IRemoteCetakWriter remoteCetakWriter, 
        IAppSettingService appSettingService, 
        IDocTypeBuilder docTypeBuilder)
    {
        _remoteCetakBuilder = remoteCetakBuilder;
        _remoteCetakWriter = remoteCetakWriter;
        _appSettingService = appSettingService;
        _docTypeBuilder = docTypeBuilder;
    }

    public Task Handle(PrintedDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var remoteAddr = _appSettingService.RemoteCetakAddress;
        var printOut = notification.Aggregate.ListDocType
            .SelectMany(hdr => hdr.ListPrintOut, (h, d) => new { h.DocTypeId, h.DocTypeName, d.PrintOutReffId })
            .FirstOrDefault(x => x.PrintOutReffId == notification.Command.ReffId);

        if (printOut is null)
            throw new KeyNotFoundException("PrintOut Reff ID not found");
        
        var docType = _docTypeBuilder.Load(new DocTypeModel(printOut.DocTypeId)).Build();

        var callbackDataOfta = new
        {
            KlaimBpjsId = notification.Aggregate.KlaimBpjsId,
            PrintOutReffId = printOut.PrintOutReffId,
            Base64Content = string.Empty
        };
        var agg = _remoteCetakBuilder
            .LoadOrCreate(new RemoteCetakModel(printOut.PrintOutReffId))
            .RemoteAddr(remoteAddr)
            .JenisDoc(docType.JenisDokRemoteCetak)
            .CallbackDataOfta(JsonSerializer.Serialize(callbackDataOfta))
            .Build();

        _remoteCetakWriter.Save(agg);
        return Task.CompletedTask;
    }
}