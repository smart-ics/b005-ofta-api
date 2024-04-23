using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsPrintDal :
    IInsertBulk<KlaimBpjsPrintModel>,
    IDelete<IKlaimBpjsKey>,
    IListData<KlaimBpjsPrintModel>
{
}