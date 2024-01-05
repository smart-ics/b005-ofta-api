using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext;

namespace Ofta.Application.DocContext.DocAgg.Workers;

public interface IDocBuilder : INunaBuilder<DocModel>
{
    IDocBuilder Create();
    IDocBuilder Load(IDocKey key);
    IDocBuilder DocType(IDocTypeKey key);
    IDocBuilder User(IUserKey key);
}
public class DocBuilder : IDocBuilder
{
    private DocModel _aggregate = new();
    private readonly IDocDal _docDal;
    private readonly IDocSigneeDal _docSigneeDal;
    private readonly IDocJurnalDal _docJurnalDal;
    private readonly IDocTypeDal _docTypeDal;
    private readonly IUserDal _userDal;
    private readonly ITglJamDal _tglJamDal;
    public DocBuilder(IDocDal docDal, 
        IDocSigneeDal docSigneeDal, 
        IDocJurnalDal docJurnalDal, 
        IDocTypeDal docTypeDal, 
        IUserDal userDal, 
        ITglJamDal tglJamDal)
    {
        _docDal = docDal;
        _docSigneeDal = docSigneeDal;
        _docJurnalDal = docJurnalDal;
        _docTypeDal = docTypeDal;
        _userDal = userDal;
        _tglJamDal = tglJamDal;
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

    public IDocBuilder DocType(IDocTypeKey key)
    {
        var docType = _docTypeDal.GetData(key)
            ?? throw new KeyNotFoundException("DocTypeId not found");
        _aggregate.DocTypeId = docType.DocTypeId;
        _aggregate.DocTypeName = docType.DocTypeName;
        return this;
    }

    public IDocBuilder User(IUserKey key)
    {
        var user = _userDal.GetData(key)
            ?? throw new KeyNotFoundException("UserId not found");
        _aggregate.UserId = user.UserId;
        _aggregate.Email = user.Email;
        return this;
    }
}