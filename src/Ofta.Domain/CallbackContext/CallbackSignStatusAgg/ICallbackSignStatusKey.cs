using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.CallbackContext.CallbackSignStatusAgg;

public interface ICallbackSignStatusKey: ITilakaNameKey
{
    string RequestId { get; }
}