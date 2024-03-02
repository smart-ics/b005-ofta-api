using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

namespace Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;

public interface IBlueprintBuilder : INunaBuilder<BlueprintModel>
{
    IBlueprintBuilder Create();
    IBlueprintBuilder Load(IBlueprintKey blueprintKey);
    IBlueprintBuilder Attach(BlueprintModel blueprint);
    IBlueprintBuilder Name(string name);
    IBlueprintBuilder AddDocType(IDocTypeKey dokTypeKey);
    IBlueprintBuilder RemoveDocType(IDocTypeKey cokTypeKey);
    IBlueprintBuilder AddSignee(IDocTypeKey docTypeKey, string email, 
        string signTag, SignPositionEnum signPosition);
    IBlueprintBuilder RemoveSignee(IDocTypeKey docTypeKey, string email);
}

public class BlueprintBuilder : IBlueprintBuilder
{
    private readonly IBlueprintDal _blueprintDal;
    private readonly IBlueprintSigneeDal _blueprintSigneeDal;
    private readonly IBlueprintDocTypeDal _blueprintDocTypeDal;
    private readonly IDocTypeDal _docTypeDal;
    private readonly IUserOftaDal _userOftaDal;
    private BlueprintModel _agg = new();

    public BlueprintBuilder(IBlueprintDal blueprintDal, 
        IBlueprintSigneeDal blueprintSigneeDal, 
        IBlueprintDocTypeDal blueprintDocTypeDal, 
        IDocTypeDal docTypeDal, 
        IUserOftaDal userOftaDal)
    {
        _blueprintDal = blueprintDal;
        _blueprintSigneeDal = blueprintSigneeDal;
        _blueprintDocTypeDal = blueprintDocTypeDal;
        _docTypeDal = docTypeDal;
        _userOftaDal = userOftaDal;
    }

    public BlueprintModel Build()
    {
        _agg.RemoveNull();
        return _agg;
    }

    public IBlueprintBuilder Create()
    {
        _agg = new BlueprintModel
        {
            ListDocType = new List<BlueprintDocTypeModel>()
        };
        return this;
    }

    public IBlueprintBuilder Load(IBlueprintKey blueprintKey)
    {
        _agg = _blueprintDal.GetData(blueprintKey)
            ?? throw new KeyNotFoundException($"Blueprint '{blueprintKey}' not found.");

        var listDocType = _blueprintDocTypeDal.ListData(blueprintKey)?.ToList()
            ?? new List<BlueprintDocTypeModel>();
        var listSignee = _blueprintSigneeDal.ListData(blueprintKey)?.ToList()
            ?? new List<BlueprintSigneeModel>();
        
        listDocType.ForEach(x =>
        {
            x.ListSignee = listSignee
                .Where(y => y.BlueprintDocTypeId == x.BlueprintDocTypeId)
                .OrderBy(y => y.NoUrut)
                .ToList();
        });
        _agg.ListDocType = listDocType;
        return this;
    }

    public IBlueprintBuilder Attach(BlueprintModel blueprint)
    {
        _agg = blueprint;
        return this;
    }

    public IBlueprintBuilder Name(string name)
    {
        _agg.BlueprintName = name;
        return this;
    }

    public IBlueprintBuilder AddDocType(IDocTypeKey docTypeKey)
    {
        var docType = _docTypeDal.GetData(docTypeKey)
            ?? throw new KeyNotFoundException($"DocType '{docTypeKey}' not found.");
        
        if (_agg.ListDocType.Any(x => x.DocTypeId == docType.DocTypeId))
            throw new ArgumentException($"DocType '{docType.DocTypeId}' already exists in blueprint.");
        
        var maxNoUrut = _agg.ListDocType
            .DefaultIfEmpty(new BlueprintDocTypeModel { NoUrut = 0 })
            .Max(x => x.NoUrut);
        maxNoUrut++;
        var blueprintDocType = new BlueprintDocTypeModel
        {
            DocTypeId = docType.DocTypeId,
            DocTypeName = docType.DocTypeName,
            NoUrut = maxNoUrut,
            ListSignee = new List<BlueprintSigneeModel>()
        };
        _agg.ListDocType.Add(blueprintDocType);
        return this;
    }

    public IBlueprintBuilder RemoveDocType(IDocTypeKey dokTypeKey)
    {
        _agg.ListDocType.RemoveAll(x => x.DocTypeId == dokTypeKey.DocTypeId);
        return this;
    }

    public IBlueprintBuilder AddSignee(IDocTypeKey docTypeKey, string email, 
        string signTag, SignPositionEnum signPosition)
    {
        var docType = _agg.ListDocType
            .FirstOrDefault(x => x.DocTypeId == docTypeKey.DocTypeId)
            ?? throw new KeyNotFoundException($"DocType '{docTypeKey}' not found.");
        
        if (docType.ListSignee.Any(x => x.Email == email))
            throw new ArgumentException($"Email '{email}' already exists in docType '{docType.DocTypeId}'.");
        
        var userOfta = _userOftaDal.GetData(email)
            ?? throw new KeyNotFoundException($"UserOfta '{email}' not found.");
        
        var maxNoUrut = docType.ListSignee
            .DefaultIfEmpty(new BlueprintSigneeModel { NoUrut = 0 })
            .Max(x => x.NoUrut);

        maxNoUrut++;
        var blueprintSignee = new BlueprintSigneeModel
        {
            Email = email,
            UserOftaId = userOfta.UserOftaId,
            NoUrut = maxNoUrut,
            SignPosition = signPosition,
            SignTag = signTag
        };
        docType.ListSignee.Add(blueprintSignee);
        return this;
    }

    public IBlueprintBuilder RemoveSignee(IDocTypeKey docTypeKey, string email)
    {
        var docType = _agg.ListDocType.FirstOrDefault(x => x.DocTypeId == docTypeKey.DocTypeId);
        if (docType is null)
            return this;
        
        docType.ListSignee.RemoveAll(x => x.Email == email);
        return this;
    }
}