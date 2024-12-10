using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class CheckUserCertificateService: ICheckUserCertificateService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;

    public CheckUserCertificateService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
    }

    public CheckUserCertificateResponse Execute(CheckUserCertificateRequest req)
    {
        var result = Task.Run(() => ExecuteCheckUserCert(req)).GetAwaiter().GetResult();
        var response = new CheckUserCertificateResponse(
            result?.Success == true,
            result?.Status ?? 0,
            result?.Message ?? new MessageDto(string.Empty),
            result?.Data ?? new List<DataDto>()
        );
        return response;
    }
    
    private async Task<CheckUserCertificateDto?> ExecuteCheckUserCert(CheckUserCertificateRequest request)
    {
        // BUILD
        var tilakaToken = await _tokenService.Execute(TilakaProviderOptions.SECTION_NAME);
        if (tilakaToken is null) 
            throw new ArgumentException($"Get tilaka token {_opt.TokenEndPoint} failed");

        var endpoint = _opt.BaseApiUrl + "/checkcertstatus";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(tilakaToken);

        var reqBody = new { user_identifier = request.TilakaName };
        var req = new RestRequest()
            .AddBody(reqBody, ContentType.Json);
        
        // EXECUTE
        var response = await client.ExecutePostAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // RETURN
        if (response.StatusCode == HttpStatusCode.Forbidden)
            return new CheckUserCertificateDto(false, 0, new MessageDto("Forbidden access to Tilaka"), null);
        
        var result = JsonSerializer.Deserialize<CheckUserCertificateDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }
    
    private record CheckUserCertificateDto(bool Success, int Status, MessageDto Message, List<DataDto>? Data);
}