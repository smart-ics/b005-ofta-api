using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public interface IKlaimBpjsMergerFileDal :
    IInsert<KlaimBpjsMergerFileModel>,
    IDelete<IKlaimBpjsKey>,
    IGetData<KlaimBpjsMergerFileModel, IKlaimBpjsKey>
{
}