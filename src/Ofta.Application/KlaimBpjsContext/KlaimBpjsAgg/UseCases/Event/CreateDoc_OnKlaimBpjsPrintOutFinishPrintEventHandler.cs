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
                .User(agg)
                .AddJurnal(DocStateEnum.Created, string.Empty)
                .Build());
        var doc = fallback.Execute(() => 
            _docBuilder.Load(new DocModel(printOut.DocId)).Build());

        // WRITE
        // doc
        doc = _docBuilder
            .Attach(doc)
            .AddJurnal(DocStateEnum.Submited, string.Empty)
            .Build();
        doc = _docWriter.Save(doc);
        
        var userSigns = JsonConvert.DeserializeObject<UserSignee>(notification.Command.User);
        if (userSigns.UserSign1 != string.Empty)
            doc = AddSigneeAndScope(doc, userSigns.UserSign1, SignPositionEnum.SignLeft);
        
        if (userSigns.UserSign2 != string.Empty)
            doc = AddSigneeAndScope(doc, userSigns.UserSign2, SignPositionEnum.SignCenter);
        
        if (userSigns.UserSign3 != string.Empty)
            doc = AddSigneeAndScope(doc, userSigns.UserSign3, SignPositionEnum.SignRight);
        
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

    private DocModel AddSigneeAndScope(DocModel doc, string user, SignPositionEnum signPosition)
    {
        var listUserOftaMapping = _userOftaMappingDal.ListData(user) ?? new List<UserOftaMappingModel>();
        var userOftaMapping = listUserOftaMapping.FirstOrDefault();

        if (userOftaMapping is null)
            return doc;
        
        var userOfta = _userOftaDal.GetData(userOftaMapping);

        if (userOfta is null)
            return doc;

        var signPositionDesc = new SignPositionDesc
        {
            UserIdentifier = userOfta.Email,
            Width = 0,
            Height = 0,
            CoordinateX = 0,
            CoordinateY = 0,
            PageNumber = 0,
            QrOption = "QRONLY"
        };

        switch (signPosition)
        {
            case SignPositionEnum.SignLeft:
                signPositionDesc.Width = _appSetting.SignPositionLeft.Width;
                signPositionDesc.Height = _appSetting.SignPositionLeft.Height;
                signPositionDesc.CoordinateX = _appSetting.SignPositionLeft.CoordinateX;
                signPositionDesc.CoordinateY = _appSetting.SignPositionLeft.CoordinateY;
                signPositionDesc.PageNumber = _appSetting.SignPositionLeft.PageNumber;
                break;
            case SignPositionEnum.SignCenter:
                signPositionDesc.Width = _appSetting.SignPositionCenter.Width;
                signPositionDesc.Height = _appSetting.SignPositionCenter.Height;
                signPositionDesc.CoordinateX = _appSetting.SignPositionCenter.CoordinateX;
                signPositionDesc.CoordinateY = _appSetting.SignPositionCenter.CoordinateY;
                signPositionDesc.PageNumber = _appSetting.SignPositionCenter.PageNumber;
                break;
            case SignPositionEnum.SignRight:
                signPositionDesc.Width = _appSetting.SignPositionRight.Width;
                signPositionDesc.Height = _appSetting.SignPositionRight.Height;
                signPositionDesc.CoordinateX = _appSetting.SignPositionRight.CoordinateX;
                signPositionDesc.CoordinateY = _appSetting.SignPositionRight.CoordinateY;
                signPositionDesc.PageNumber = _appSetting.SignPositionRight.PageNumber;
                break;
        }
        

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
    public string UserSign1 { get; set; }
    public string UserSign2 { get; set; }
    public string UserSign3 { get; set; }
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