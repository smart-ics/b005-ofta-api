using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;

namespace Ofta.Application.PrintOutContext.RemoteCetakAgg.Contracts;

public interface IRemoteCetakDal :
    IInsert<RemoteCetakModel>,
    IUpdate<RemoteCetakModel>,
    IGetData<RemoteCetakModel, IRemoteCetakKey>,
    IDelete<IRemoteCetakKey>,
    IListData<RemoteCetakModel, IRemoteCetakKey>,
    IListData<RemoteCetakModel, Periode>
{
    
}
    
    
