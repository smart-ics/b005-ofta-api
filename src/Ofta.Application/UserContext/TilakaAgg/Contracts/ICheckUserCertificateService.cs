using System.Text.Json.Serialization;
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

public class DataDto {
    [JsonPropertyName("status")]
    public string Status { get; set; }
   
    [JsonPropertyName("serialnumber")]
    public string SerialNumber { get; set; }
   
    [JsonPropertyName("subject_dn")]
    public string SubjectDn { get; set; }

    [JsonPropertyName("start_active_date")]
    public string StartActiveDate { get; set; }

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; }

    [JsonPropertyName("certificate")]
    public string Certificate { get; set; }
}