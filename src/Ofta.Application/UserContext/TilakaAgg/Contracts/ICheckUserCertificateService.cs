using iTextSharp.text;
using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface ICheckUserCertificateService: INunaService<CheckUserCertificateResponse, CheckUserCertificateRequest>
{
}

public record CheckUserCertificateRequest(string TilakaName);

public record CheckUserCertificateResponse(
    bool Success,
    int Status,
    MessageDto Message,
    List<DataDto> Data
);

public record MessageDto(string Info);

public record DataDto(
    string Status,
    string Serialnumber,
    string SubjectDn,
    string StartActiveDate,
    string ExpiryDate,
    string Certificate
);