using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface ICheckExistingAccountService: INunaService<CheckExistingAccountResponse, CheckExistingAccountRequest>
{
}

public record CheckExistingAccountRequest(string RegistrationId, string NomorIdentitas): ITilakaRegistrationKey;

public record CheckExistingAccountResponse(
    bool Status,
    string Message,
    string TilakaId
);