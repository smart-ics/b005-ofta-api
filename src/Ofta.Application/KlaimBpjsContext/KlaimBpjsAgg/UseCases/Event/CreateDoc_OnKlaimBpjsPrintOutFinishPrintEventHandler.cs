using System.Reflection;
using MediatR;
using Newtonsoft.Json;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class CreateDoc_OnKlaimBpjsPrintOutFinishPrintEventHandler 
    : INotificationHandler<FinishedPrintDocKlaimBpjsEvent>
{
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;
    private readonly IKlaimBpjsWriter _klaimBpjsWriter;
    private readonly IWriteFileService _writeFileService;
    private readonly IUserOftaMappingDal _userOftaMappingDal;

    public CreateDoc_OnKlaimBpjsPrintOutFinishPrintEventHandler(IDocBuilder docBuilder, 
        IDocWriter docWriter, 
        IKlaimBpjsWriter klaimBpjsWriter, 
        IWriteFileService writeFileService, IUserOftaMappingDal userOftaMappingDal)
    {
        _docBuilder = docBuilder;
        _docWriter = docWriter;
        _klaimBpjsWriter = klaimBpjsWriter;
        _writeFileService = writeFileService;
        _userOftaMappingDal = userOftaMappingDal;
    }

    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        //  create Document atas file yg sudah di-print
        var agg = notification.Agg;
        var cmd = notification.Command;

        var printOut = agg.ListDocType
            .SelectMany(x => x.ListPrintOut)
            .FirstOrDefault(x => x.PrintOutReffId == cmd.PrintOutReffId)
            ?? throw new ArgumentException("Document not found");

        var docType = agg.ListDocType
            .First(x => x.ListPrintOut.Any(y => y.PrintOutReffId == cmd.PrintOutReffId));

        var fallback = Policy<DocModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _docBuilder
                .Create()
                .DocType(new DocTypeModel(docType.DocTypeId))
                .User(agg)
                .AddJurnal(DocStateEnum.Created, string.Empty)
                .Build());
        var doc = fallback.Execute(() => 
            _docBuilder.Load(new DocModel(printOut.DocId)).Build());

        //  WRITE
        //      doc
        doc = _docBuilder
            .Attach(doc)
            .AddJurnal(DocStateEnum.Submited, string.Empty)
            .Build();
        
        var userSigns = JsonConvert.DeserializeObject<UserSignee>(notification.Command.User);
        if (userSigns?.UserSign1 is not null)
        {
            var userOfta = _userOftaMappingDal
                .ListData(userSigns.UserSign1)
                .First();
            
            doc = _docBuilder
                .Attach(doc)
                .AddSignee(userOfta, "", SignPositionEnum.SignLeft, "", "")
                .AddScope(userOfta)
                .Build();
        }
        
        if (userSigns?.UserSign2 is not null)
        {
            var userOfta = _userOftaMappingDal
                .ListData(userSigns.UserSign2)
                .First();
            
            doc = _docBuilder
                .Attach(doc)
                .AddSignee(userOfta, "", SignPositionEnum.SignCenter, "", "")
                .AddScope(userOfta)
                .Build();
        }
        
        if (userSigns?.UserSign3 is not null)
        {
            var userOfta = _userOftaMappingDal
                .ListData(userSigns.UserSign3)
                .First();
            
            doc = _docBuilder
                .Attach(doc)
                .AddSignee(userOfta, "", SignPositionEnum.SignRight, "", "")
                .AddScope(userOfta)
                .Build();
        }
        
        doc = _docWriter.Save(doc);
        doc = _docBuilder.Attach(doc)
            .GenRequestedDocUrl()
            .Build();
        doc = _docWriter.Save(doc);

        //      klaim
        printOut.DocId = doc.DocId;
        printOut.DocUrl = doc.RequestedDocUrl;
        _klaimBpjsWriter.Save(agg);
        
        //      save fisik file;
        var writeFileRequest = new WriteFileRequest(doc.RequestedDocUrl, cmd.Base64Content);
        _ = _writeFileService.Execute(writeFileRequest);

        return Task.CompletedTask;
    }
}

public class UserSignee
{
    public string UserSign1 { get; set; }
    public string UserSign2 { get; set; }
    public string UserSign3 { get; set; }
}