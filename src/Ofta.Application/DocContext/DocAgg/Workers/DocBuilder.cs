using System.Globalization;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.ParamContext.SystemAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.Workers;

public interface IDocBuilder : INunaBuilder<DocModel>
{
    IDocBuilder Create();
    IDocBuilder Load(IDocKey key);
    IDocBuilder Attach(DocModel model);
    IDocBuilder DocType(IDocTypeKey key);
    IDocBuilder User(IUserOftaKey oftaKey);
    IDocBuilder DocState(DocStateEnum docStateEnum, string description);
    IDocBuilder GenRequestedDocUrl();
    IDocBuilder GenPublishedDocUrl();
    IDocBuilder AddSignee(IUserOftaKey userOftaKey, string signTag, SignPositionEnum signPositionEnum);
    IDocBuilder RemoveSignee(IUserOftaKey userOftaKey);
    IDocBuilder UploadedDocId(string uploadedDocId);
    IDocBuilder Sign(string email);
    IDocBuilder RejectSign(string email);
    IDocBuilder UploadedDocUrl(string uploadedDocUrl);
}
public class DocBuilder : IDocBuilder
{
    private DocModel _aggregate = new();
    private readonly IDocDal _docDal;
    private readonly IDocSigneeDal _docSigneeDal;
    private readonly IDocJurnalDal _docJurnalDal;
    private readonly IDocTypeDal _docTypeDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly ITglJamDal _tglJamDal;
    private readonly IParamSistemDal _paramSistemDal;

    public DocBuilder(IDocDal docDal, 
        IDocSigneeDal docSigneeDal, 
        IDocJurnalDal docJurnalDal, 
        IDocTypeDal docTypeDal, 
        IUserOftaDal userDal, 
        ITglJamDal tglJamDal, 
        IParamSistemDal paramSistemDal)
    {
        _docDal = docDal;
        _docSigneeDal = docSigneeDal;
        _docJurnalDal = docJurnalDal;
        _docTypeDal = docTypeDal;
        _userOftaDal = userDal;
        _tglJamDal = tglJamDal;
        _paramSistemDal = paramSistemDal;
    }

    public DocModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public IDocBuilder Create()
    {
        _aggregate = new DocModel
        {
            DocDate = _tglJamDal.Now,
            DocState = DocStateEnum.Created,
            ListJurnal = new List<DocJurnalModel>(),
            ListSignees = new List<DocSigneeModel>()
        };
        return this;
    }

    public IDocBuilder Load(IDocKey key)
    {
        _aggregate = _docDal.GetData(key)
            ?? throw new KeyNotFoundException("DocId not found");
        _aggregate.ListSignees = _docSigneeDal.ListData(key)?.ToList()
            ?? new List<DocSigneeModel>();
        _aggregate.ListJurnal = _docJurnalDal.ListData(key)?.ToList()
            ?? new List<DocJurnalModel>();
        return this;
    }

    public IDocBuilder Attach(DocModel model)
    {
        _aggregate = model;
        return this;
    }

    public IDocBuilder DocType(IDocTypeKey key)
    {
        var docType = _docTypeDal.GetData(key)
            ?? throw new KeyNotFoundException("DocTypeId not found");
        _aggregate.DocTypeId = docType.DocTypeId;
        _aggregate.DocTypeName = docType.DocTypeName;
        return this;
    }

    public IDocBuilder User(IUserOftaKey oftaKey)
    {
        var user = _userOftaDal.GetData(oftaKey)
            ?? throw new KeyNotFoundException("UserId not found");
        _aggregate.UserOftaId = user.UserOftaId;
        _aggregate.Email = user.Email;
        return this;
    }

    public IDocBuilder DocState(DocStateEnum docStateEnum, string description)
    {
        _aggregate.DocState = docStateEnum;
        var noUrut = _aggregate.ListJurnal.DefaultIfEmpty(new DocJurnalModel{NoUrut = 1})
            .Max(x => x.NoUrut);
        noUrut++;
        var desc = docStateEnum.ToString();
        desc += description.Length != 0 ? description : string.Empty;
        var jurnal = new DocJurnalModel
        {
            NoUrut = noUrut,
            JurnalDate = _tglJamDal.Now,
            DocState = docStateEnum,
            JurnalDesc = desc, 
        };
        _aggregate.ListJurnal.Add(jurnal);
        return this;
    }

    public IDocBuilder GenRequestedDocUrl()
    {
        var storageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var docTypeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_aggregate.DocTypeName);
        var requestedDocUrl = $"{storageUrl.ParamSistemValue}/{_aggregate.DocId}_{docTypeName}.pdf";
        _aggregate.RequestedDocUrl = requestedDocUrl;
        return this;
    }

    public IDocBuilder GenPublishedDocUrl()
    {
        var oftaStorageUrl = _paramSistemDal.GetData(Sys.OftaStorageUrl)
            ?? throw new KeyNotFoundException("'Sys.OftaStoragePath' not found");
        var docTypeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_aggregate.DocTypeName);
        var publishedDocUrl = $"{oftaStorageUrl.ParamSistemValue}/{_aggregate.DocId}_{docTypeName}.pdf";
        _aggregate.PublishedDocUrl = publishedDocUrl;
        return this;
    }

    public IDocBuilder AddSignee(IUserOftaKey userOftaKey, string signTag, SignPositionEnum signPosition)
    {
        var userOfta = _userOftaDal.GetData(userOftaKey)
                       ?? throw new KeyNotFoundException("User Ofta not found");
        
        _aggregate.ListSignees.RemoveAll(x => x.SignPosition == signPosition);
        _aggregate.ListSignees.Add(new DocSigneeModel
        {
            UserOftaId = userOftaKey.UserOftaId,
            Email = userOfta.Email,
            SignTag = signTag,
            SignPosition = signPosition,
            SignedDate = new DateTime(3000,1,1),
            SignState = SignStateEnum.NotSigned,
            Level = 1,
        });
        return this;
    }

    public IDocBuilder RemoveSignee(IUserOftaKey userOftaKey)
    {
        _aggregate.ListSignees.RemoveAll(x => x.UserOftaId == userOftaKey.UserOftaId);
        return this;
    }

    public IDocBuilder UploadedDocId(string uploadedDocId)
    {
        _aggregate.UploadedDocId = uploadedDocId;
        return this;
    }

    public IDocBuilder Sign(string email)
    {
        var signedUser = _aggregate.ListSignees.FirstOrDefault(x => x.Email == email)
            ?? throw new KeyNotFoundException($"Email {email} is not a signee");
        if (signedUser.SignState == SignStateEnum.Signed)
            throw new ArgumentException("User has already signed");
        signedUser.SignState = SignStateEnum.Signed;
        signedUser.SignedDate = _tglJamDal.Now;
        return this;
    }

    public IDocBuilder RejectSign(string email)
    {
        var rejectedUser = _aggregate.ListSignees.FirstOrDefault(x => x.Email == email)
            ?? throw new KeyNotFoundException($"Email {email} is not a signee");
        rejectedUser.SignState = SignStateEnum.Rejected;
        rejectedUser.SignedDate = _tglJamDal.Now;
        return this;
        
    }

    public IDocBuilder UploadedDocUrl(string uploadedDocUrl)
    {
        _aggregate.UploadedDocUrl = uploadedDocUrl;
        return this;
    }
}