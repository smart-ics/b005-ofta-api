using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.UserContext.TilakaAgg.Contracts;

public interface IRequestRevokeService: INunaService<RequestRevokeResponse, RequestRevokeRequest>
{
}

public record RequestRevokeRequest(string TilakaName, string Reason);

public record RequestRevokeResponse(
    bool Success, 
    string Message,
    string RevokeToken,
    string UrlWebview
);