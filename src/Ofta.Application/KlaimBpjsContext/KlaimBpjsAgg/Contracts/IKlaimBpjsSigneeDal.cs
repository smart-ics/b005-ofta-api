using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsSigneeDal :
    IInsertBulk<KlaimBpjsSigneeModel>,
    IDelete<IKlaimBpjsKey>,
    IListData<KlaimBpjsSigneeModel, IKlaimBpjsKey>
{
}