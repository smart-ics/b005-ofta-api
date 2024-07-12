using iTextSharp.text;
using iTextSharp.text.pdf;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.ParamContext.SystemAgg;
using Polly;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;


public class CreateDoc_OnKlaimBpjsMergerFileEventHandler
    : INotificationHandler<MergerFileBpjsEvent>
{
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;
    private readonly IKlaimBpjsWriter _klaimBpjsWriter;
    private readonly IParamSistemDal _paramSistemDal;

    public CreateDoc_OnKlaimBpjsMergerFileEventHandler(IDocBuilder docBuilder,
        IDocWriter docWriter,
        IKlaimBpjsWriter klaimBpjsWriter,
        IParamSistemDal paramSistemDal)
    {
        _docBuilder = docBuilder;
        _docWriter = docWriter;
        _klaimBpjsWriter = klaimBpjsWriter;
        _paramSistemDal = paramSistemDal;
    }

    public Task Handle(MergerFileBpjsEvent notification, CancellationToken cancellationToken)
    {

        var agg = notification.Agg;
        var cmd = notification.Command;

        // GUARD
        var paramStoragePath = _paramSistemDal.GetData(Sys.LocalStoragePath)
            ?? throw new KeyNotFoundException("Parameter StoragePath not found");
        var paramStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");

        var printOuts = agg.ListDocType
            .SelectMany(x => x.ListPrintOut)
            .Where(printOut => !string.IsNullOrEmpty(printOut.DocUrl?.Trim())).ToList();

        if (!printOuts.Any())
        {
            throw new ArgumentException("Document not found");
        }

        // BUILD
        var fallback = Policy<DocModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _docBuilder
                .Create()
                .DocType(new DocTypeModel("DTX00"))
                .User(agg)
                .AddJurnal(DocStateEnum.Created, string.Empty)
                .Build());
        var doc = fallback.Execute(() =>
            _docBuilder.Load(new DocModel(agg.MergerDocId)).Build());

        // WRITE
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
        agg.MergerDocId = doc.DocId;
        agg.MergerDocUrl = doc.RequestedDocUrl;
        _klaimBpjsWriter.Save(agg);

        //      mergerFile;       
        var files = agg.ListDocType
            .SelectMany(x => x.ListPrintOut)
            .Where(printOut => !string.IsNullOrEmpty(printOut.DocUrl?.Trim()))
            .Select(printOut => printOut.DocUrl.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue))
            .ToList();

        var result = agg.MergerDocUrl.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue);
        MergePdfFiles(files, result);

        return Task.CompletedTask;
    }

    static void MergePdfFiles(List<string> pdfFiles, string outputPdf)
    {
        using (FileStream stream = new FileStream(outputPdf, FileMode.Create))
        {
            Document document = new Document();
            PdfCopy pdf = new PdfCopy(document, stream);
            PdfReader reader = null;
            document.Open();

            try
            {
                foreach (string file in pdfFiles)
                {
                    reader = new PdfReader(file);
                    pdf.AddDocument(reader);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exception here
                if (reader != null)
                {
                    reader.Close();
                }
                throw new Exception("Error during PDF merge process", ex);
            }
            finally
            {
                if (document != null)
                {
                    document.Close();
                }
            }
        }
    }
}
