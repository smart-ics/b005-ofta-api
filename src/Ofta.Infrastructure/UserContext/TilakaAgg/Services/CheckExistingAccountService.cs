using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class CheckExistingAccountService : ICheckExistingAccountService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;

    public CheckExistingAccountService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService)
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

    private async Task<CheckExistingAccountDto?> ExecuteCheckExistingAccount(CheckExistingAccountRequest request)
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

        // RETURN
        if (response.StatusCode == HttpStatusCode.Forbidden)
            return new CheckExistingAccountDto
            {
                TilakaId = string.Empty, 
                Message = "Forbidden access to Tilaka",
                Status = false
            };
        
        var result = JsonConvert.DeserializeObject<CheckExistingAccountDto>(response.Content ?? string.Empty);
        return result;
    }

    private class CheckExistingAccountDto
    {
        [JsonProperty("tilaka_id")]
        public string TilakaId;

        [JsonProperty("message")]
        public string Message;

        [JsonProperty("status")]
        public bool Status;
    }
}