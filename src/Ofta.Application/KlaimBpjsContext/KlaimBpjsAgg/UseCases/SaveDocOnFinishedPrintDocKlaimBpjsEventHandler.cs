using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Polly;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public class SaveDocOnFinishedPrintDocKlaimBpjsEventHandler 
    : INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;
    private readonly IKlaimBpjsWriter _klaimBpjsWriter;
    private readonly ISaveFileService _saveFileService;

    public SaveDocOnFinishedPrintDocKlaimBpjsEventHandler(IDocBuilder docBuilder, 
        IDocWriter docWriter, 
        IKlaimBpjsWriter klaimBpjsWriter, 
        ISaveFileService saveFileService)
    {
        _docBuilder = docBuilder;
        _docWriter = docWriter;
        _klaimBpjsWriter = klaimBpjsWriter;
        _saveFileService = saveFileService;
    }

    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        //  create Document atas file yg sudah di-print
        var klaimBpjs = notification.Agg;
        var cmd = notification.Command;
        var itemKlaim = klaimBpjs.ListDoc.FirstOrDefault(x => x.PrintOutReffId == cmd.PrintOutReffId);
        if (itemKlaim is null)
            throw new ArgumentException("Document not found");

        var fallback = Policy<DocModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _docBuilder
                .Create()
                .DocType(new DocTypeModel(itemKlaim.DocTypeId))
                .User(klaimBpjs)
                .AddJurnal(DocStateEnum.Created, string.Empty)
                .Build());
        var doc = fallback.Execute(() => 
            _docBuilder.Load(new DocModel(itemKlaim.DocId)).Build());

        //  WRITE
        //      doc
        doc = _docBuilder
            .Attach(doc)
            .AddJurnal(DocStateEnum.Submited, string.Empty)
            .Build();
        doc = _docWriter.Save(doc);
        doc = _docBuilder.Attach(doc)
            .GenRequestedDocUrl()
            .Build();
        doc = _docWriter.Save(doc);
        
        //      klaim
        itemKlaim.DocId = doc.DocId;
        _klaimBpjsWriter.Save(klaimBpjs);
        
        //      save fisik file;
        var saveDocFileRequest = new SaveDocFileRequest
        {
            FilePathName = doc.RequestedDocUrl,
            FileContentBase64 = cmd.Base64Content
        };
        _saveFileService.Execute(saveDocFileRequest);
        return Task.CompletedTask;        
    }
}