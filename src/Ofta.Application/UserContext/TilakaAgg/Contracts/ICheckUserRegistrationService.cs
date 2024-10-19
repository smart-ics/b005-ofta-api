using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface ICheckUserRegistrationService: INunaService<CheckUserRegistrationResponse, CheckUserRegistrationRequest>
{
}

public record CheckUserRegistrationRequest(string RegistrationId): ITilakaRegistrationKey;

public record CheckUserRegistrationResponse(
    bool Success, 
    string Message,
    string TilakaName,
    string RegistrationStatus,
    string ManualRegistrationStatus
);