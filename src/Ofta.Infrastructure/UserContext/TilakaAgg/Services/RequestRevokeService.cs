using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class RequestRevokeService: IRequestRevokeService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;

    public RequestRevokeService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
    }

    public RequestRevokeResponse Execute(RequestRevokeRequest req)
    {
        var result = Task.Run(() => ExecuteRequestRevoke(req)).GetAwaiter().GetResult();
        var data = result?.Data ?? new List<string>();
        var response = new RequestRevokeResponse(
            result?.Success == true,
            result?.Message ?? string.Empty,
            data.Count > 0 ? data.First() : string.Empty,
            data.Count > 0 ? data.Last() : string.Empty
        );
        return response;
    }

    private async Task<RequestRevokeDto?> ExecuteRequestRevoke(RequestRevokeRequest request)
    {
        // BUILD
        var tilakaToken = await _tokenService.Execute(TilakaProviderOptions.SECTION_NAME);
        if (tilakaToken is null)
            throw new ArgumentException($"Get tilaka token {_opt.TokenEndPoint} failed");

        var endpoint = _opt.BaseApiUrl + "/requestRevokeCertificate";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(tilakaToken);

        var reqBody = new
        {
            user_identifier = request.TilakaName,
            reason = request.Reason,
        };

        var req = new RestRequest()
            .AddBody(reqBody, ContentType.Json);

        // EXECUTE
        var response = await client.ExecutePostAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // RETURN
        var result = JsonSerializer.Deserialize<RequestRevokeDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }

    private record RequestRevokeDto(bool Success, string Message, List<string>? Data);
}