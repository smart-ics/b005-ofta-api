using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.PrintOutContext.ICasterAgg;
using static iTextSharp.text.pdf.AcroFields;


namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueIcaster_OnKlaimBpjsPrintOutPrintedEventHandler : INotificationHandler<PrintedDocKlaimBpjsEvent>
{

    private readonly ISendToICasterService _sendToICasterService;
    private readonly IDocTypeBuilder _docTypeBuilder;


    public AddQueueIcaster_OnKlaimBpjsPrintOutPrintedEventHandler(ISendToICasterService sendToICasterService,
                                                                  IDocTypeBuilder docTypeBuilder  )
    {
        _sendToICasterService = sendToICasterService;
        _docTypeBuilder = docTypeBuilder;
    }

    public Task Handle(PrintedDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        var printOut = notification.Aggregate.ListDocType
            .SelectMany(hdr => hdr.ListPrintOut, (h, d) => new { h.DocTypeId, h.DocTypeName, d.PrintOutReffId })
            .Where(x => x.DocTypeId == notification.Command.DocTypeId)
            .FirstOrDefault(x => x.PrintOutReffId == notification.Command.ReffId);

        if (printOut is null)
            throw new KeyNotFoundException("PrintOut Reff ID not found");

        var docType = _docTypeBuilder.Load(new DocTypeModel(printOut.DocTypeId)).Build();

        if (string.IsNullOrWhiteSpace(docType?.DocTypeCode))
        {
            // Tidak mengirim ke emrService jika DocTypeCode kosong
            return Task.CompletedTask;
        }

        if (string.IsNullOrEmpty(docType?.DocTypeCode))
        {
            // Tidak mengirim ke emrService jika DocTypeCode kosong
            return Task.CompletedTask;
        }

        var reqObj = new ICasterModel
        {
            FromUser = "ofta",
            ToUser = "emrPrintService",
            Message = new MessageModel
            {
                KlaimBpjsId = notification.Aggregate.KlaimBpjsId,
                PrintOutReffId = printOut.PrintOutReffId,
                Base64Content = string.Empty,
                Type = docType.JenisDokRemoteCetak,
                RegId = notification.Aggregate.RegId
            }
        };

        _sendToICasterService.Execute(reqObj);
            
        return Task.FromResult(reqObj.Message.KlaimBpjsId);
        
    }
}