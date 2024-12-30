using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp.Authenticators;
using RestSharp;
using System.Text.Json;


namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;

public class ExecuteSignTilakaService : IExecuteSignToSignProviderService
{ 
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _token;

    public ExecuteSignTilakaService(IOptions<TilakaProviderOptions> options, ITokenTilakaService token)
    {
        _opt = options.Value;
        _token = token;
    }

    public ExecuteSignToSignProviderResponse Execute(ExecuteSignToSignProviderRequest req)
    {
        var data = Task.Run(() => ExecuteSign(req)).GetAwaiter().GetResult();
        var result = new ExecuteSignToSignProviderResponse { Status = data?.Success == true , Message = data?.Message ?? string.Empty };
        return result;
    }
    
    private async Task<ExecuteSignToTilakaResponse?> ExecuteSign(ExecuteSignToSignProviderRequest request)
    {
        //  BUILD REQUEST
        var token = await _token.Execute("TilakaProvider");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        var payload = new
        {
            request_id = request.Signee.DocSigneeId,
            user_identifier = request.UserProvider
        };
        var signJson = JsonSerializer.Serialize(payload);
        
        var options = new RestClientOptions(_opt.UploadEndpoint)
        {
            Authenticator = new JwtAuthenticator(token)
        };
        
        var client = new RestClient(options);
        var req = new RestRequest("/executesign")
            .AddJsonBody(signJson);

        //  EXECUTE
        var response = await client.ExecutePostAsync(req);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var resp = JsonSerializer.Deserialize<ExecuteSignToTilakaResponse>(response.Content ?? string.Empty, jsonOptions);

        //  RETURN
        return resp;
    }
    
    private record ExecuteSignToTilakaResponse(bool Success, string Message, string Status);
}
