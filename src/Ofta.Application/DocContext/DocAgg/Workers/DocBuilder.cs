using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Application.DocContext.DocAgg.Workers;

public interface IDocBuilder : INunaBuilder<DocModel>
{
    IDocBuilder Create();
    IDocBuilder Load(IDocKey key);
    IDocBuilder DocType(IDocTypeKey key);
    IDocBuilder User(IUserOftaKey oftaKey);
    IDocBuilder DocState(DocStateEnum docStateEnum);
    IDocBuilder AddSignee(IUserOftaKey userOftaKey, string signTag, SignPositionEnum signPositionEnum);
    IDocBuilder RemoveSignee(IUserOftaKey userOftaKey);
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

    public DocBuilder(IDocDal docDal, 
        IDocSigneeDal docSigneeDal, 
        IDocJurnalDal docJurnalDal, 
        IDocTypeDal docTypeDal, 
        IUserOftaDal userDal, 
        ITglJamDal tglJamDal)
    {
        _docDal = docDal;
        _docSigneeDal = docSigneeDal;
        _docJurnalDal = docJurnalDal;
        _docTypeDal = docTypeDal;
        _userOftaDal = userDal;
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

    public IDocBuilder User(IUserOftaKey oftaKey)
    {
        var user = _userOftaDal.GetData(oftaKey)
            ?? throw new KeyNotFoundException("UserId not found");
        _aggregate.UserOftaId = user.UserOftaId;
        _aggregate.Email = user.Email;
        return this;
    }

    public IDocBuilder DocState(DocStateEnum docStateEnum)
    {
        _aggregate.DocState = docStateEnum;
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
            IsSigned = false,
            Level = 1,
        });
        return this;
    }

    public IDocBuilder RemoveSignee(IUserOftaKey userOftaKey)
    {
        _aggregate.ListSignees.RemoveAll(x => x.UserOftaId == userOftaKey.UserOftaId);
        return this;
    }
}