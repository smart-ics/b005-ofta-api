using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface IRegisterUserTilakaService: INunaService<RegisterUserTilakaResponse, RegisterUserTilakaRequest>
{
}

public record RegisterUserTilakaRequest(TilakaUserModel Model);

public record RegisterUserTilakaResponse(bool Success, string Message, string RegisterId);