using Ofta.Domain.ParamContext.ConnectionAgg;

namespace Ofta.Application.ParamContext.ConnectionAgg.Contracts;

public interface IDbConnectionDal 
{
    DataBaseConnModel Get();
}
