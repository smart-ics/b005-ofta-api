using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;

public class GetSignatureInfoService: IGetSignatureInfoService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;

    public GetSignatureInfoService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
    }

    public GetSignatureInfoResponse Execute(GetSignatureInfoRequest req)
    {
        var result = Task.Run(() => ExecuteGetSignatureInfo(req)).GetAwaiter().GetResult();
        var signers = result?.Signers.Select(x => new SignerResponse(x.TilakaName, x.Datetime, x.Reason, x.Location)).ToList();
        var response = new GetSignatureInfoResponse(
            result?.Success == true,
            result?.Message ?? string.Empty,
            result?.FileName ?? string.Empty,
            signers ?? new List<SignerResponse>()
        );
        return response;
    }

    private async Task<GetSignatureInfoDto?> ExecuteGetSignatureInfo(GetSignatureInfoRequest request)
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
        var req = new RestRequest("/getsignatureinfo")
            .AddQueryParameter("id", request.DocumentId);

        // EXECUTE
        var response = await client.ExecuteGetAsync(req);
        if (response.StatusCode != HttpStatusCode.OK)
            return null;

        // RETURN
        var result = JsonConvert.DeserializeObject<GetSignatureInfoDto>(response.Content ?? string.Empty);
        return result;
    }
    
    private class SignerDto
    {
        [JsonProperty("tilakaname")]
        public string TilakaName;

        [JsonProperty("datetime")]
        public string Datetime;

        [JsonProperty("reason")]
        public string Reason;
        
        [JsonProperty("location")]
        public string Location;
    }
    
    private class GetSignatureInfoDto
    {
        [JsonProperty("success")]
        public bool Success;

        [JsonProperty("message")]
        public string Message;

        [JsonProperty("filename")]
        public string FileName;
        
        [JsonProperty("signers")]
        public IEnumerable<SignerDto> Signers;
    }
}