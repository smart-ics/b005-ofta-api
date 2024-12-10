using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg.Service;

public class ExecuteBulkSignService: IExecuteBulkSignService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _token;

    public ExecuteBulkSignService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService token)
    {
        _opt = opt.Value;
        _token = token;
    }

    public ExecuteBulkSignResponse Execute(ExecuteBulkSignRequest req)
    {
        var result = Task.Run(() => ExecuteAsync(req)).GetAwaiter().GetResult();
        return new ExecuteBulkSignResponse(result?.Success == true, result?.Message ?? string.Empty);;
    }
    
    private async Task<ExecuteBulkSignDto?> ExecuteAsync(ExecuteBulkSignRequest request)
    {
        //  BUILD REQUEST
        var token = await _token.Execute(TilakaProviderOptions.SECTION_NAME);
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        var endpoint = _opt.UploadEndpoint + "/executesign";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);

        var payload = new
        {
            request_id = request.BulkSign.BulkSignId,
            user_identifier = request.UserProvider
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var req = new RestRequest
        {
            Method = Method.Post
        };
        req.AddJsonBody(jsonPayload);

        //  EXECUTE
        var response = await client.ExecuteAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // RETURN
        var result = JsonSerializer.Deserialize<ExecuteBulkSignDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }
    
    private record ExecuteBulkSignDto(bool Success, string Message);
}