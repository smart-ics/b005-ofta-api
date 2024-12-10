using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public interface IGetSignatureInfoService: INunaService<GetSignatureInfoResponse, GetSignatureInfoRequest>
{
}

public record GetSignatureInfoRequest(string DocumentId);

public record GetSignatureInfoResponse(
    bool Success,
    string Message,
    string Filename,
    List<SignerResponse> Signers
);

public record SignerResponse(string TilakaName, string DateTime, string Reason, string Location): ITilakaNameKey;