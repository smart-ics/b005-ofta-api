using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class CheckExistingAccountService: ICheckExistingAccountService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenService _tokenService;

    public CheckExistingAccountService(IOptions<TilakaProviderOptions> opt, ITokenService tokenService)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
    }

    public CheckExistingAccountResponse Execute(CheckExistingAccountRequest req)
    {
        var result = Task.Run(() => ExecuteCheckExistingAccount(req)).GetAwaiter().GetResult();
        var response = new CheckExistingAccountResponse(
            result?.Status ?? false,
            result?.Message ?? string.Empty,
            result?.TilakaId ?? string.Empty
        );
        return response;
    }

    private async Task<CheckExistingAccountResponse?> ExecuteCheckExistingAccount(CheckExistingAccountRequest request)
    {
        // BUILD
        var tilakaToken = await _tokenService.Execute(TilakaProviderOptions.SECTION_NAME);
        if (tilakaToken is null)
            throw new ArgumentException($"Get tilaka token {_opt.TokenEndPoint} failed");
        
        var endpoint = _opt.BaseApiUrl + "/checkAkunDSExist";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(tilakaToken);

        var reqBody = new
        {
            request_id = request.RegistrationId,
            nik = request.NomorIdentitas
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
        var result = JsonSerializer.Deserialize<CheckExistingAccountResponse>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }
}