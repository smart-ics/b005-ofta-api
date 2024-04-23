using System.Text.Json;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public class AddQueueRemoteCetakOnPrintedDocKlaimBpjsEventHandler : INotificationHandler<PrintedDocKlaimBpjsEvent>
{
    private readonly IRemoteCetakBuilder _remoteCetakBuilder;
    private readonly IRemoteCetakWriter _remoteCetakWriter;
    private readonly IAppSettingService _appSettingService;
    private readonly IDocTypeBuilder _docTypeBuilder;

    public AddQueueRemoteCetakOnPrintedDocKlaimBpjsEventHandler(IRemoteCetakBuilder remoteCetakBuilder, 
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
        var doc = notification.Aggregate.ListDocType.FirstOrDefault(x => x.NoUrut == notification.Command.NoUrut);
        if (doc is null)
            throw new ArgumentException("Document to be printed not found");
        var docType = _docTypeBuilder.Load(doc).Build();

        var callbackDataOfta = new
        {
            KlaimBpjsId = notification.Aggregate.KlaimBpjsId,
            PrintOutReffId = doc.PrintOutReffId,
            Base64Content = string.Empty
        };
        var agg = _remoteCetakBuilder
            .LoadOrCreate(new RemoteCetakModel(doc.PrintOutReffId))
            .RemoteAddr(remoteAddr)
            .JenisDoc(docType.JenisDokRemoteCetak)
            .CallbackDataOfta(JsonSerializer.Serialize(callbackDataOfta))
            .Build();

        _remoteCetakWriter.Save(agg);
        return Task.CompletedTask;
    }
}