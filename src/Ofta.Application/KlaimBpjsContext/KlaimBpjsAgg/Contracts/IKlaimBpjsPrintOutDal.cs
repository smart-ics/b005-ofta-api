using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsPrintOutDal :
    IInsertBulk<KlaimBpjsPrintOutModel>,
    IDelete<IKlaimBpjsKey>,
    IGetData<KlaimBpjsPrintOutModel, IDocKey>,
    IListData<KlaimBpjsPrintOutModel, IKlaimBpjsKey>
{
}