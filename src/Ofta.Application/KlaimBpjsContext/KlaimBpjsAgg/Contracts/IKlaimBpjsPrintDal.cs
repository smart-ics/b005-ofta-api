using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsPrintDal :
    IInsertBulk<KlaimBpjsPrintOutModel>,
    IDelete<IKlaimBpjsKey>,
    IListData<KlaimBpjsPrintOutModel>
{
}