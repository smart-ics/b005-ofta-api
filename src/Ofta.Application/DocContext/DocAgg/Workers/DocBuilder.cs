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
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.Workers;

public interface IDocBuilder : INunaBuilder<DocModel>
{
    IDocBuilder Create();
    IDocBuilder DocDate(DateTime date);
    IDocBuilder Load(IDocKey key);
    IDocBuilder Load(string uploadedDocId);
    IDocBuilder Attach(DocModel model);
    IDocBuilder DocType(IDocTypeKey key);
    IDocBuilder User(IUserOftaKey oftaKey);
    IDocBuilder DocName(string name);
    IDocBuilder GenRequestedDocUrl();
    IDocBuilder GenRequestedMergedDocUrl(string mergerName);
    IDocBuilder GenPublishedDocUrl();
    IDocBuilder AddSignee(IUserOftaKey userOftaKey, string signTag, SignPositionEnum signPositionEnum, 
                          string signPositionDesc,string signUrl, bool isHidden = true);
    IDocBuilder RemoveSignee(IUserOftaKey userOftaKey);
    IDocBuilder UploadedDocId(string uploadedDocId);
    IDocBuilder Sign(string email);
    IDocBuilder RejectSign(string email);
    IDocBuilder UploadedDocUrl(string uploadedDocUrl);
    IDocBuilder AddJurnal(DocStateEnum docStateEnum, string description);

    IDocBuilder AddScope<T>(T scope)
        where T : IScope;
    IDocBuilder RemoveScope<T>(T scope)
        where T : IScope;
}

public class DocBuilder : IDocBuilder
{
    private DocModel _aggregate = new();
    private readonly IDocDal _docDal;
    private readonly IDocSigneeDal _docSigneeDal;
    private readonly IDocJurnalDal _docJurnalDal;
    private readonly IDocScopeDal _docScopeDal;
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
        IParamSistemDal paramSistemDal, 
        IDocScopeDal docScopeDal)
    {
        _docDal = docDal;
        _docSigneeDal = docSigneeDal;
        _docJurnalDal = docJurnalDal;
        _docTypeDal = docTypeDal;
        _userOftaDal = userDal;
        _tglJamDal = tglJamDal;
        _paramSistemDal = paramSistemDal;
        _docScopeDal = docScopeDal;
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
            ListSignees = new List<DocSigneeModel>(),
            ListScope = new List<AbstractDocScopeModel>()
        };
        
        var jurnal = new DocJurnalModel
        {
            NoUrut = 1,
            JurnalDate = _tglJamDal.Now,
            DocState = DocStateEnum.Created,
            JurnalDesc = "Doc Created"
        };
        _aggregate.ListJurnal.Add(jurnal);
        return this;
    }

    public IDocBuilder DocDate(DateTime date)
    {
        _aggregate.DocDate = date;
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
        _aggregate.ListScope = _docScopeDal.ListData(key)?.ToList()
            ?? new List<AbstractDocScopeModel>();
        return this;
    }

    public IDocBuilder Load(string uploadedDocId)
    {
        var key = new DocModel
        {
            UploadedDocId = uploadedDocId
        };
        
        _aggregate = _docDal.GetData((IUploadedDocKey) key)
            ?? throw new KeyNotFoundException("DocId not found");
        _aggregate.ListSignees = _docSigneeDal.ListData(new DocModel(_aggregate.DocId))?.ToList()
            ?? new List<DocSigneeModel>();
        _aggregate.ListJurnal = _docJurnalDal.ListData(new DocModel(_aggregate.DocId))?.ToList()
            ?? new List<DocJurnalModel>();
        _aggregate.ListScope = _docScopeDal.ListData(new DocModel(_aggregate.DocId))?.ToList()
            ?? new List<AbstractDocScopeModel>();
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

    public IDocBuilder DocName(string name)
    {
        _aggregate.DocName = name;
        return this;
    }
    public IDocBuilder GenRequestedDocUrl()
    {
        var storageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var docTypeName = CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(_aggregate.DocTypeName)
            .Replace(" ", "_");
        var requestedDocUrl = $"{storageUrl.ParamSistemValue}/{_aggregate.DocId}_{docTypeName}.pdf";
        _aggregate.RequestedDocUrl = requestedDocUrl;
        return this;
    }

    public IDocBuilder GenPublishedDocUrl()
    {
        var oftaStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("'Sys.OftaStoragePath' not found");
        var docTypeName = CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(_aggregate.DocTypeName)
            .Replace(" ", "_");
        var publishedDocUrl = $"{oftaStorageUrl.ParamSistemValue}/{_aggregate.DocId}_{docTypeName}.pdf";
        _aggregate.PublishedDocUrl = publishedDocUrl;
        return this;
    }

    public IDocBuilder GenRequestedMergedDocUrl(string mergerName)
    {
        var storageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var docTypeName = CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(_aggregate.DocTypeName)
            .Replace(" ", "_");
        var requestedDocUrl = $"{storageUrl.ParamSistemValue}/{mergerName}.pdf";
        _aggregate.RequestedDocUrl = requestedDocUrl;
        return this;
    }


    public IDocBuilder AddSignee(IUserOftaKey userOftaKey, string signTag, SignPositionEnum signPosition, 
                                 string signPositionDesc, string signUrl, bool isHidden = true)
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
            SignPositionDesc = signPositionDesc,
            SignUrl = signUrl,
            IsHidden = true
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

    public IDocBuilder AddJurnal(DocStateEnum docStateEnum, string description)
    {
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
            JurnalDesc = desc
        };
        _aggregate.DocState = docStateEnum;
        _aggregate.ListJurnal.Add(jurnal);
        return this;
    }

    public IDocBuilder AddScope<T>(T scope) where T : IScope
    {
        switch (scope)
        {
            case IUserOftaKey userKey:
                AddScopeUser(userKey);
                break;
            case ITeamKey teamKey:
                AddScopeTeam(teamKey);
                break;
        }
        return this;
        
        void AddScopeUser(IUserOftaKey userKey)
        {
            //  jika sudah ada, abaikan
            if (_aggregate.ListScope.OfType<DocScopeUserModel>()
                .Any(item => item.UserOftaId == userKey.UserOftaId))
                return;
                
            //  add scope
            var newUser = new DocScopeUserModel()
            {
                ScopeType = 0,
                UserOftaId = userKey.UserOftaId
            };
            _aggregate.ListScope.Add(newUser);        
        }
        void AddScopeTeam(ITeamKey teamKey)
        {
            //  jika sudah ada, abaikan
            if (_aggregate.ListScope.OfType<DocScopeTeamModel>()
                .Any(item => item.TeamId == teamKey.TeamId))
                return;
            //  add scope
            var newTeam = new DocScopeTeamModel()
            {
                ScopeType = 1,
                TeamId = teamKey.TeamId
            };
            _aggregate.ListScope.Add(newTeam);
        }    
    }
    
    public IDocBuilder RemoveScope<T>(T scope) where T : IScope
    {
        switch (scope)
        {
            case IUserOftaKey scopeUser:
                _aggregate.ListScope.RemoveAll(x => 
                    x.GetType() == typeof(DocScopeUserModel) &&
                    ((DocScopeUserModel)x).UserOftaId == scopeUser.UserOftaId);
                break;
            case ITeamKey scopeTeam:
                _aggregate.ListScope.RemoveAll(x => 
                    x.GetType() == typeof(DocScopeTeamModel) &&
                    ((DocScopeTeamModel)x).TeamId == scopeTeam.TeamId);
                break;
        }
        return this;
    }
}