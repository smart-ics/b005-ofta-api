using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg.Service;

public class CheckBulkSignStatusService: ICheckBulkSignStatusService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _token;

    public CheckBulkSignStatusService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService token)
    {
        _opt = opt.Value;
        _token = token;
    }

    public CheckBulkSignStatusResponse Execute(CheckBulkSignStatusRequest req)
    {
        var checkSign = Task.Run(() => ExecuteAsync(req)).GetAwaiter().GetResult();
        var listDocument = checkSign?.ListPdf.Select(x => new FileItemResponse(x.Filename, x.PresignedUrl)).ToList();
        var response = new CheckBulkSignStatusResponse(checkSign?.Success == true, checkSign?.Message ?? string.Empty, listDocument ?? new List<FileItemResponse>());
        return response;
    }
    
    private async Task<CheckBulkSignStatusDto?> ExecuteAsync(CheckBulkSignStatusRequest request)
    {
        // BUILD REQUEST
        var token = await _token.Execute(TilakaProviderOptions.SECTION_NAME);
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        var endpoint = _opt.UploadEndpoint + "/checksignstatus";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);

        var payload = new
        {
            request_id = request.BulkSign.BulkSignId
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var req = new RestRequest
        {
            Method = Method.Post,
        };
        req.AddJsonBody(jsonPayload);

        // EXECUTE
        var response = await client.ExecuteAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        //  RETURN
        var result = JsonSerializer.Deserialize<CheckBulkSignStatusDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }
    
}