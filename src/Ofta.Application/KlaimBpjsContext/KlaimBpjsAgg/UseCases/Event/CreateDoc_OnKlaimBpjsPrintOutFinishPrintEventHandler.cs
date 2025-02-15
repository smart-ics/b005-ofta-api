using MediatR;
using Newtonsoft.Json;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.Helpers;
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
    private readonly IAppSettingService _appSetting;
    private readonly IDocBuilder _docBuilder;
    private readonly IDocWriter _docWriter;
    private readonly IKlaimBpjsWriter _klaimBpjsWriter;
    private readonly IWriteFileService _writeFileService;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IUserOftaMappingDal _userOftaMappingDal;

    public CreateDoc_OnKlaimBpjsPrintOutFinishPrintEventHandler(IAppSettingService appSetting, IDocBuilder docBuilder, IDocWriter docWriter, IKlaimBpjsWriter klaimBpjsWriter, IWriteFileService writeFileService, IUserOftaDal userOftaDal, IUserOftaMappingDal userOftaMappingDal)
    {
        _appSetting = appSetting;
        _docBuilder = docBuilder;
        _docWriter = docWriter;
        _klaimBpjsWriter = klaimBpjsWriter;
        _writeFileService = writeFileService;
        _userOftaDal = userOftaDal;
        _userOftaMappingDal = userOftaMappingDal;
    }
    
    public Task Handle(FinishedPrintDocKlaimBpjsEvent notification, CancellationToken cancellationToken)
    {
        // BUILD
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
                .DocName(cmd.DocName)
                .User(agg)
                .AddJurnal(DocStateEnum.Created, string.Empty)
                .Build());
        var doc = fallback.Execute(() => 
            _docBuilder.Load(new DocModel(printOut.DocId)).Build());

        // WRITE
        // doc
        doc = _docBuilder
            .Attach(doc)
            .DocName(cmd.DocName)
            .AddJurnal(DocStateEnum.Submited, string.Empty)
            .Build();
        doc = _docWriter.Save(doc);
        
        var userSignees = JsonConvert.DeserializeObject<UserSignee>(notification.Command.User);
        if (userSignees != null)
        {
            if (userSignees.UserSign1.UserId != string.Empty)
                doc = AddSigneeAndScope(doc, userSignees.UserSign1, SignPositionEnum.SignLeft);
        
            if (userSignees.UserSign2.UserId != string.Empty)
                doc = AddSigneeAndScope(doc, userSignees.UserSign2, SignPositionEnum.SignCenter);
        
            if (userSignees.UserSign3.UserId != string.Empty)
                doc = AddSigneeAndScope(doc, userSignees.UserSign3, SignPositionEnum.SignRight);
        }
        
        doc = _docBuilder.Attach(doc)
            .GenRequestedDocUrl()
            .Build();
        doc = _docWriter.Save(doc);

        // klaim
        printOut.DocId = doc.DocId;
        printOut.DocUrl = doc.RequestedDocUrl;
        _klaimBpjsWriter.Save(agg);
        
        // save fisik file;
        var writeFileRequest = new WriteFileRequest(doc.RequestedDocUrl, cmd.Base64Content);
        _ = _writeFileService.Execute(writeFileRequest);

        return Task.CompletedTask;
    }

    private DocModel AddSigneeAndScope(DocModel doc, UserSigneeDto user, SignPositionEnum signPosition)
    {
        var listUserOftaMapping = _userOftaMappingDal.ListData(user.UserId) ?? new List<UserOftaMappingModel>();
        var userOftaMapping = listUserOftaMapping.FirstOrDefault();

        if (userOftaMapping is null)
            return doc;
        
        var userOfta = _userOftaDal.GetData(userOftaMapping);

        if (userOfta is null)
            return doc;

        var signPositionDesc = new SignPositionDesc
        {
            UserIdentifier = userOfta.Email,
            Width = _appSetting.SignPositionLeft.Width,
            Height = _appSetting.SignPositionLeft.Height,
            CoordinateX = user.SignPosition.X,
            CoordinateY = user.SignPosition.Y,
            PageNumber = user.SignPosition.PageNumber,
            QrOption = "QRONLY"
        };

        var signPositionDescJson = JsonConvert.SerializeObject(signPositionDesc);
            
        doc = _docBuilder
            .Attach(doc)
            .AddSignee(userOfta, "Mengetahui", signPosition, signPositionDescJson, "")
            .AddScope(userOfta)
            .Build();
        
        doc.ListSignees.First().IsHidden = false;
        return doc;
    }
}

public class UserSignee
{
    public UserSigneeDto UserSign1 { get; set; }
    public UserSigneeDto UserSign2 { get; set; }
    public UserSigneeDto UserSign3 { get; set; }
}

public class UserSigneeDto
{
    public string UserId { get; set; }
    public UserSigneePosition SignPosition { get; set; }
}

public class UserSigneePosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public int PageNumber { get; set; }
}

internal class SignPositionDesc
{
    [JsonProperty("user_identifier")]
    public string UserIdentifier { get; set; }
        
    [JsonProperty("width")]
    public int Width { get; set; }
        
    [JsonProperty("height")]
    public int Height { get; set; }
            
    [JsonProperty("coordinate_x")]
    public int CoordinateX { get; set; }
        
    [JsonProperty("coordinate_y")]
    public int CoordinateY { get; set; }
        
    [JsonProperty("page_number")]
    public int PageNumber { get; set; }
        
    [JsonProperty("qr_option")]
    public string QrOption { get; set; }
}