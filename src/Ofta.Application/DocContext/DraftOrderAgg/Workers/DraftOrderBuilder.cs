using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.DocContext.DraftOrderAgg;

namespace Ofta.Application.DocContext.DraftOrderAgg.Workers;

public interface IDraftOrderBuilder : INunaBuilder<DraftOrderModel>
{
    IDraftOrderBuilder Create();
    IDraftOrderBuilder DrafterUser(string drafterUserId);
    IDraftOrderBuilder RequesterUser(string requesterUserId);
    IDraftOrderBuilder DocType(IDocTypeKey key);
    IDraftOrderBuilder Context(string contextName, string contextReffId);
}
public class DraftOrderBuilder: IDraftOrderBuilder
{
    private DraftOrderModel _aggregate = new();
    private readonly ITglJamDal _tglJamDal;
    private readonly IDocTypeDal _docTypeDal;

    public DraftOrderBuilder(ITglJamDal tglJamDal, IDocTypeDal docTypeDal)
    {
        _tglJamDal = tglJamDal;
        _docTypeDal = docTypeDal;
    }

    public DraftOrderModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public IDraftOrderBuilder Create()
    {
        _aggregate = new DraftOrderModel
        {
            DraftOrderDate = _tglJamDal.Now
        };
        return this;
    }

    public IDraftOrderBuilder DrafterUser(string drafterUserId)
    {
        _aggregate.DrafterUserId = drafterUserId;
        return this;
    }

    public IDraftOrderBuilder RequesterUser(string requesterUserId)
    {
        _aggregate.RequesterUserId = requesterUserId;
        return this;
    }

    public IDraftOrderBuilder DocType(IDocTypeKey key)
    {
        var docType = _docTypeDal.GetData(key)
            ?? throw new KeyNotFoundException($"DocTypeId: {key.DocTypeId} not found");
        _aggregate.DocTypeId = docType.DocTypeId;
        _aggregate.DocTypeName = docType.DocTypeName;
        return this;
    }

    public IDraftOrderBuilder Context(string contextName, string contextReffId)
    {
        _aggregate.Context = contextName;
        _aggregate.ContextReffId = contextReffId;
        return this;
    }
}