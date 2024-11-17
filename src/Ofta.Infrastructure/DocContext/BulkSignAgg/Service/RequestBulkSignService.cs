using System.Text.Json;
using System.Text.Json.Serialization;
using iText.Layout.Element;
using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.DocContext.BulkSignAgg;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg.Service;

public class RequestBulkSignService: IRequestBulkSignService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _token;
    private readonly ITilakaUserBuilder _tilakaUserBuilder;

    public RequestBulkSignService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService token, ITilakaUserBuilder tilakaUserBuilder)
    {
        _opt = opt.Value;
        _token = token;
        _tilakaUserBuilder = tilakaUserBuilder;
    }

    public ReqBulkSignResponse Execute(ReqBulkSignRequest req)
    {
        throw new NotImplementedException();
    }

    private async Task<RequestBulkSignDto?> ExecuteAsync(ReqBulkSignRequest request)
    {
        var payload = GenerateRequestBulkSignPayload(request.BulkSign);
        var jsonPaylaod = JsonSerializer.Serialize(payload);
        
        // BUILD
        var token = await _token.Execute(TilakaProviderOptions.SECTION_NAME);
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");
        
        // var payload = GenerateRequestBulkSignPayload(request.BulkSign);
        // var jsonPaylaod = JsonSerializer.Serialize(payload);
        
        var endpoint = _opt.UploadEndpoint + "/requestsign";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);
        
        var req = new RestRequest
        {
            Method = Method.Post,
        };
        req.AddJsonBody(jsonPaylaod);
        
        // EXECUTE
        var response = await client.ExecuteAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        // RETURN
        var result = JsonSerializer.Deserialize<RequestBulkSignDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }

    private RequestBulkSignPayload GenerateRequestBulkSignPayload(BulkSignModel bulkSign)
    {
        var listAllSignee = new List<SignatureDto>();
        var listPdf = new List<FileDto>();
        bulkSign.ListDoc.ForEach(doc =>
        {
            var listSigneeEachDoc = doc.ListSignee
                .Where(signee => !string.IsNullOrWhiteSpace(signee.SignPositionDesc))
                .Select(signee =>
                {
                    var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(signee.SignPositionDesc);
                    var newSignee = new SignatureDto
                    {
                        UserIdentifier = signPositionDescJson?.GetValueOrDefault("user_identifier").GetString() ?? string.Empty,
                        Reason = _opt.Reason,
                        Location = _opt.Location,
                        Width = signPositionDescJson?.GetValueOrDefault("width").GetDouble() ?? 0,
                        Height = signPositionDescJson?.GetValueOrDefault("height").GetDouble() ?? 0,
                        CoordinateX = signPositionDescJson?.GetValueOrDefault("coordinate_x").GetDouble() ?? 0,
                        CoordinateY = signPositionDescJson?.GetValueOrDefault("coordinate_y").GetDouble() ?? 0,
                        PageNumber = signPositionDescJson?.GetValueOrDefault("page_number").GetInt32() ?? 0,
                        QrOption = signPositionDescJson?.GetValueOrDefault("qr_option").GetString() ?? string.Empty
                    };

                    var tilakaUser = _tilakaUserBuilder
                        .Load(newSignee.UserIdentifier)
                        .Build();

                    newSignee.UserIdentifier = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;
                    return newSignee;
                }).ToList();
            
            listPdf.Add(new FileDto(doc.UploadedDocId, listSigneeEachDoc));
            listAllSignee.AddRange(listSigneeEachDoc);
        });

        return new RequestBulkSignPayload(bulkSign.BulkSignId, listAllSignee, listPdf);
    }
    
    private record RequestBulkSignPayload(string RequestId, List<SignatureDto> Signatures, List<FileDto> ListPdf);

    private record FileDto(string Filename, List<SignatureDto> Signatures);

    private class SignatureDto
    {
        [JsonPropertyName("user_identifier")]
        public string UserIdentifier { get; set; }
        
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
        
        [JsonPropertyName("location")]
        public string Location { get; set; }
        
        [JsonPropertyName("width")]
        public double Width { get; set; }
        
        [JsonPropertyName("height")]
        public double Height { get; set; }
            
        [JsonPropertyName("coordinate_x")]
        public double CoordinateX { get; set; }
        
        [JsonPropertyName("coordinate_y")]
        public double CoordinateY { get; set; }
        
        [JsonPropertyName("page_number")]
        public int PageNumber { get; set; }
        
        [JsonPropertyName("qr_option")]
        public string QrOption { get; set; }
    }
    
    private class RequestBulkSignDto
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("auth_urls")]
        public List<AuthUrlDto> AuthUrls { get; set; }
        
        [JsonPropertyName("failed_doc_name")]
        public List<string> FailedDocName { get; set; }
    }

    private class AuthUrlDto
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("user_identifier")]
        public string UserIdentifier { get; set; }
    }
}