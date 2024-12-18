using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.CallbackContext.CallbackCertificateStatusAgg;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.Contracts;

public interface ICallbackCertificateStatusDal:
    IInsert<CallbackCertificateStatusModel>, 
    IUpdate<CallbackCertificateStatusModel>,
    IGetData<CallbackCertificateStatusModel, ITilakaRegistrationKey>
{
}