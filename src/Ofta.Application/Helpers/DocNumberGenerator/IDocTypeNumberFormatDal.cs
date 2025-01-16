using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.Helpers.DocNumberGenerator;

public interface IDocTypeNumberFormatDal: 
    IInsert<DocTypeNumberFormatModel>,
    IUpdate<DocTypeNumberFormatModel>,
    IDelete<IDocTypeKey>, 
    IGetData<DocTypeNumberFormatModel, IDocTypeKey>
{
}