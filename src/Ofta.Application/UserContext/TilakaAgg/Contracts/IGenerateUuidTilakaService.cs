using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface IGenerateUuidTilakaService: INunaService<GenerateUuidTilakaResponse>
{
}

public record GenerateUuidTilakaRequest();

public record GenerateUuidTilakaResponse(bool Success, string Message, string RegistrationId): ITilakaRegistrationKey;