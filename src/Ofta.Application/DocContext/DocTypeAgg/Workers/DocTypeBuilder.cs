using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Workers;

public interface IDocTypeBuilder : INunaBuilder<DocTypeModel>
{
    IDocTypeBuilder Create();
    IDocTypeBuilder Load(IDocTypeKey key);
    IDocTypeBuilder Name(string name);
    IDocTypeBuilder IsActive(bool isActive);
    IDocTypeBuilder Template(string templateUrl);
}

public class DocTypeBuilder : IDocTypeBuilder
{
    private DocTypeModel _aggregate = new();
    private readonly IDocTypeDal _docTypeDal;
    private readonly IDocTypeTagDal _docTypeTagDal;

    public DocTypeBuilder(IDocTypeDal docTypeDal, 
        IDocTypeTagDal docTypeTagDal)
    {
        _docTypeDal = docTypeDal;
        _docTypeTagDal = docTypeTagDal;
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
        _aggregate = _docTypeDal.GetData(key)
                     ?? throw new KeyNotFoundException("DocType not found");
        _aggregate.ListTag = _docTypeTagDal.ListData(key)?.ToList()
                             ?? new List<DocTypeTagModel>();
        return this;
    }

    public IDocTypeBuilder Name(string name)
    {
        _aggregate.DocTypeName = name;
        return this;
    }

    public IDocTypeBuilder IsActive(bool isActive)
    {
        _aggregate.IsActive = isActive;
        return this;
    }

    public IDocTypeBuilder Template(string templateUrl)
    {
        _aggregate.TemplateUrl = templateUrl;
        //  get extension from url
        var ext = Path.GetExtension(templateUrl);
        _aggregate.TemplateType = ext.ToLower() switch
        {
            ".html" => TemplateTypeEnum.Html,
            ".docx" or ".doc" => TemplateTypeEnum.Word,
            _ => throw new InvalidOperationException("Invalid template file type")
        };
        return this;
    }
}