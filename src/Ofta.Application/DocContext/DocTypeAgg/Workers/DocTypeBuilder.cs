using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.ParamContext.SystemAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Workers;

public interface IDocTypeBuilder : INunaBuilder<DocTypeModel>
{
    IDocTypeBuilder Create();
    IDocTypeBuilder Load(IDocTypeKey key);
    IDocTypeBuilder Name(string name);
    IDocTypeBuilder DocTypeCode(string code);
    IDocTypeBuilder IsActive(bool isActive);
    IDocTypeBuilder AddTag(string tag);
    IDocTypeBuilder RemoveTag(string tag);
    IDocTypeBuilder FileUrl(string fileUrl);
    IDocTypeBuilder DefaultDrafter(IUserOftaKey key);
}

public class DocTypeBuilder : IDocTypeBuilder
{
    private DocTypeModel _aggregate = new();
    private readonly IDocTypeDal _docTypeDal;
    private readonly IDocTypeTagDal _docTypeTagDal;
    private readonly IParamSistemDal _paramSistemDal;
    private readonly IUserOftaDal _userOftaDal;

    public DocTypeBuilder(IDocTypeDal docTypeDal, 
        IDocTypeTagDal docTypeTagDal,
        IParamSistemDal paramSistemDal, IUserOftaDal userOftaDal)
    {
        _docTypeDal = docTypeDal;
        _docTypeTagDal = docTypeTagDal;
        _paramSistemDal = paramSistemDal;
        _userOftaDal = userOftaDal;
    }

    public DocTypeModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public IDocTypeBuilder Create()
    {
        _aggregate = new DocTypeModel
        {
            ListTag = new List<DocTypeTagModel>()
        };
        return this;
    }

    public IDocTypeBuilder Load(IDocTypeKey key)
    {

        var templateUrl = _paramSistemDal.GetData(Sys.LocalTemplateUrl)
                       ?? throw new KeyNotFoundException("Parameter StorageUrl not found");


        _aggregate = _docTypeDal.GetData(key)
                     ?? throw new KeyNotFoundException("DocType not found");

        _aggregate.FileUrl = $"{templateUrl.ParamSistemValue}/{_aggregate.FileUrl}";

        _aggregate.ListTag = _docTypeTagDal.ListData(key)?.ToList()
                             ?? new List<DocTypeTagModel>();

        return this;
    }

    public IDocTypeBuilder Name(string name)
    {
        _aggregate.DocTypeName = name;
        return this;
    }

    public IDocTypeBuilder DocTypeCode(string code)
    {
        _aggregate.DocTypeCode = code;
        return this;
    }

    public IDocTypeBuilder IsActive(bool isActive)
    {
        _aggregate.IsActive = isActive;
        return this;
    }

    public IDocTypeBuilder AddTag(string tag)
    {
        tag = tag.ToLower();
        
        if (tag.Contains(' '))
            throw new InvalidOperationException("Tag cannot contain space");

        _aggregate.ListTag.RemoveAll(x => x.Tag == tag);
        _aggregate.ListTag.Add(new DocTypeTagModel
        {
            Tag = tag
        });
        return this;
    }

    public IDocTypeBuilder RemoveTag(string tag)
    {
        tag = tag.ToLower();
        _aggregate.ListTag.RemoveAll(x => x.Tag == tag);
        return this;
    }

    public IDocTypeBuilder FileUrl(string fileUrl)
    {
        _aggregate.FileUrl = fileUrl;
        return this;
    }

    public IDocTypeBuilder DefaultDrafter(IUserOftaKey key)
    {
        var user = _userOftaDal.GetData(key)
            ?? throw new KeyNotFoundException($"User ofta {key.UserOftaId} not found");

        _aggregate.DefaultDrafterUserId = user.UserOftaId;
        return this;
    }
}