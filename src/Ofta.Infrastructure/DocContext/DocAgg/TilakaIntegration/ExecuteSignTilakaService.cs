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
        var result = new ExecuteSignToSignProviderResponse { Status = data?.Succes ?? string.Empty,Message = data?.Message ?? string.Empty };
        return result;
    }
    #region Execute Sign
    private async Task<ExecuteSignToTilakaResponse?> ExecuteSign(ExecuteSignToSignProviderRequest request)
    {
        //  BUILD REQUEST
        var fileUrl = request.Doc.RequestedDocUrl;
        var token = await _token.Execute("TilakaProvider");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        var endpoint = _opt.UploadEndpoint + "/executesign";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);

        var jsonBody = new
        {
            request_id = request.Doc.DocId,
            user_identifier = request.UserOfta
        };

        var req = new RestRequest()
            .AddJsonBody(jsonBody);


        req.Method = Method.Post;

        //  EXECUTE
        var response = await client.ExecuteAsync(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var resp = JsonSerializer.Deserialize<ExecuteSignToTilakaResponse>(response.Content ?? string.Empty, jsonOptions);

        //  RETURN
        return resp;
    }
    #endregion


    #region RESPONSE COMMAND
    private record ExecuteSignToTilakaResponse(string Succes, string Message);
    #endregion
}
