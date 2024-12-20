using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class GenerateUuidTilakaService : IGenerateUuidTilakaService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;

    public GenerateUuidTilakaService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
    }

    public GenerateUuidTilakaResponse Execute()
    {
        var result = Task.Run(ExecuteGenerateUuid).GetAwaiter().GetResult();
        var response = new GenerateUuidTilakaResponse(
            result?.Success == true,
            result?.Message ?? string.Empty,
            result?.Data is not null ? result.Data.First() : string.Empty
        );
        return response;
    }

    private async Task<GenerateUuidTilakaDto?> ExecuteGenerateUuid()
    {
        // BUILD
        var tilakaToken = await _tokenService.Execute(TilakaProviderOptions.SECTION_NAME);
        if (tilakaToken is null)
            throw new ArgumentException($"Get tilaka token {_opt.TokenEndPoint} failed");

        var options = new RestClientOptions(_opt.BaseApiUrl)
        {
            Authenticator = new JwtAuthenticator(tilakaToken)
        };
        
        var client = new RestClient(options);
        var req = new RestRequest("/generateUUID");

        // EXECUTE
        var response = await client.ExecutePostAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // RETURN
        if (response.StatusCode == HttpStatusCode.Forbidden)
            return new GenerateUuidTilakaDto(false, "Forbidden access to Tilaka", null);
        
        var result = JsonSerializer.Deserialize<GenerateUuidTilakaDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }

    private record GenerateUuidTilakaDto(bool Success, string Message, List<string>? Data);
}