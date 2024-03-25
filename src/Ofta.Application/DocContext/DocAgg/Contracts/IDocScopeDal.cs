using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public interface IDocScopeDal :
    IInsertBulk<AbstractDocScopeModel>,
    IDelete<IDocKey>,
    IListData<AbstractDocScopeModel, IDocKey>
{
}