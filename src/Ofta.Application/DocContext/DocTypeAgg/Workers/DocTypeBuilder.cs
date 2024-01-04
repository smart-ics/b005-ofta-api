using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Workers;

public interface IDocTypeBuilder : INunaBuilder<DocTypeModel>
{
    IDocTypeBuilder Create();
    IDocTypeBuilder Name(string name);
    IDocTypeBuilder IsActive(bool isActive);
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
}