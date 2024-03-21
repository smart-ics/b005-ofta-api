using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsEventDal :
    IInsertBulk<KlaimBpjsEventModel>,
    IDelete<IKlaimBpjsKey>,
    IListData<KlaimBpjsEventModel, IKlaimBpjsKey>
{
}
