using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class GetSignatureImageService: IGetSignatureImageService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;

    public GetSignatureImageService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
    }

    public GetSignatureImageResponse Execute(GetSignatureImageRequest req)
    {
        var result = Task.Run(() => ExecuteGetSignatureImage(req)).GetAwaiter().GetResult();
        var response = new GetSignatureImageResponse(
            result?.Success == true,
            result?.Message ?? string.Empty,
            new ImageResponse(result?.Data?.SignatureBase64 ?? string.Empty, result?.Data?.Name ?? string.Empty)
        );
        return response;
    }
    
    private async Task<GetSignatureImageDto?> ExecuteGetSignatureImage(GetSignatureImageRequest request)
    {
        // BUILD
        var tilakaToken = await _tokenService.Execute(TilakaProviderOptions.SECTION_NAME);
        if (tilakaToken is null) 
            throw new ArgumentException($"Get tilaka token {_opt.TokenEndPoint} failed");

        var endpoint = _opt.BaseApiUrl + "/signing-getsignature";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(tilakaToken);

        var req = new RestRequest()
            .AddQueryParameter("user_identifier", request.TilakaName);
        
        // EXECUTE
        var response = await client.ExecutePostAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // RETURN
        if (response.StatusCode == HttpStatusCode.Forbidden)
            return new GetSignatureImageDto(false, "Forbidden access to Tilaka", null);
        
        var result = JsonSerializer.Deserialize<GetSignatureImageDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }

    private record GetSignatureImageDto(bool Success, string Message, ImageDto? Data);
    
    private class ImageDto
    {
        [JsonPropertyName("signature_base64")]
        public string SignatureBase64 { get; set; }
   
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}