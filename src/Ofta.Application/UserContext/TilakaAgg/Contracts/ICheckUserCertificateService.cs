using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface ICheckUserCertificateService: INunaService<CheckUserCertificateResponse, CheckUserCertificateRequest>
{
}

public record CheckUserCertificateRequest(string TilakaName);

public record CheckUserCertificateResponse(
    bool Success,
    string Message,
    int Status
);