using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsDocTypeDal :
    IInsertBulk<KlaimBpjsDocTypeModel>,
    IDelete<IKlaimBpjsKey>,
    IListData<KlaimBpjsDocTypeModel, IKlaimBpjsKey>
{
}