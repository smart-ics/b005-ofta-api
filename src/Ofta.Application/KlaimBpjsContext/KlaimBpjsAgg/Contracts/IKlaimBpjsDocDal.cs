using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsDocDal :
    IInsertBulk<KlaimBpjsDocModel>,
    IDelete<IKlaimBpjsKey>,
    IListData<KlaimBpjsDocModel, IKlaimBpjsKey>
{
}