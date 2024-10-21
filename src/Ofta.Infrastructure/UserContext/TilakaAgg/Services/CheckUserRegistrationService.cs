using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class CheckUserRegistrationService: ICheckUserRegistrationService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;

    public CheckUserRegistrationService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
    }

    public CheckUserRegistrationResponse Execute(CheckUserRegistrationRequest req)
    {
        var result = Task.Run(() => ExecuteCheckUserReg(req)).GetAwaiter().GetResult();
        var response = new CheckUserRegistrationResponse(
            result?.Success == true,
            result?.Message ?? string.Empty,
            result?.Data.TilakaName ?? string.Empty,
            result?.Data.Status ?? string.Empty,
            result?.Data.ManualRegistrationStatus ?? string.Empty,
            result?.Data.PhotoSelfie ?? string.Empty,
            result?.Data.ReasonCode ?? string.Empty
        );
        return response;
    }

    private async Task<CheckUserRegistrationDto?> ExecuteCheckUserReg(CheckUserRegistrationRequest request)
    {
        // BUILD
        var tilakaToken = await _tokenService.Execute(TilakaProviderOptions.SECTION_NAME);
        if (tilakaToken is null) 
            throw new ArgumentException($"Get tilaka token {_opt.TokenEndPoint} failed");

        var endpoint = _opt.BaseApiUrl + "/userregstatus";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(tilakaToken);

        var reqBody = new { register_id = request.RegistrationId };
        var req = new RestRequest()
            .AddBody(reqBody, ContentType.Json);
        
        // EXECUTE
        var response = await client.ExecutePostAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // RETURN
        var result = JsonSerializer.Deserialize<CheckUserRegistrationDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }

    private record CheckUserRegistrationDto(bool Success, string Message, CheckUserRegistrationData Data);

    private class CheckUserRegistrationData
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("photo_selfie")]
        public string PhotoSelfie { get; set; }
        
        [JsonPropertyName("tilaka_name")]
        public string TilakaName { get; set; }

        [JsonPropertyName("reason_code")] 
        public string ReasonCode { get; set; }
        
        [JsonPropertyName("manual_registration_status")]
        public string ManualRegistrationStatus { get; set; }
    }
}