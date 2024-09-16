using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.PrintOutContext.ICasterAgg;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class AddQueueICasterOnKlaimBpjsPrintOutScannReffidEventHandler : INotificationHandler<ScannReffidKlaimBpjsEvent>
{

    private readonly ISendToICasterService _sendToICasterService;
    private readonly IDocTypeBuilder _docTypeBuilder;


    public AddQueueICasterOnKlaimBpjsPrintOutScannReffidEventHandler(ISendToICasterService sendToICasterService,
                                                                      IDocTypeBuilder docTypeBuilder)
    {
        _sendToICasterService = sendToICasterService;
        _docTypeBuilder = docTypeBuilder;
    }

    public Task Handle(ScannReffidKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        
        var printOut = notification.Aggregate.ListDocType
            .SelectMany(hdr => hdr.ListPrintOut, (h, d) => new { h.DocTypeId, h.DocTypeName, d.PrintOutReffId });
        
        foreach (var item in printOut)
        {
            var docTypeModel = new DocTypeModel(item.DocTypeId);
            var docType = _docTypeBuilder.Load(docTypeModel).Build();
            if (string.IsNullOrEmpty(docType?.DocTypeCode))
                continue;

            if (string.IsNullOrWhiteSpace(docType?.DocTypeCode))
                continue;

            var reqObj = new ICasterModel
            {
                FromUser = "ofta",
                ToUser = "emrPrintService",
                Message = new MessageModel
                {
                    KlaimBpjsId = notification.Aggregate.KlaimBpjsId,
                    PrintOutReffId = item.PrintOutReffId,
                    Base64Content = string.Empty,
                    Type = docType.JenisDokRemoteCetak,
                    RegId = notification.Aggregate.RegId
                }
            };
                
            _sendToICasterService.Execute(reqObj);
            
            
        }

        return Task.CompletedTask;

    }
}
