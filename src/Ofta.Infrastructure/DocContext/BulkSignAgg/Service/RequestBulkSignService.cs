using System.Text.Json;
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
        var result = Task.Run(() => ExecuteAsync(req)).GetAwaiter().GetResult();
        if (result?.AuthUrls is not null)
            req.BulkSign.ListDoc = UpdateAuthUrl(req, result);

        if (result?.FailedDocName is not null)
            req.BulkSign.ListDoc = UpdateRequestBulkSignState(req, result);
        
        return new ReqBulkSignResponse(result?.Success == true, result?.Message ?? string.Empty, req.BulkSign);
    }

    private async Task<RequestBulkSignResponseDto?> ExecuteAsync(ReqBulkSignRequest request)
    {
        // BUILD
        var token = await _token.Execute(TilakaProviderOptions.SECTION_NAME);
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");
        
        var payload = GenerateRequestBulkSignPayload(request.BulkSign);
        var jsonPayload = JsonSerializer.Serialize(payload);
        
        var endpoint = _opt.UploadEndpoint + "/requestsign";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);
        
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
        
        // RETURN
        var result = JsonSerializer.Deserialize<RequestBulkSignResponseDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }

    private RequestSignPayload GenerateRequestBulkSignPayload(BulkSignModel bulkSign)
    {
        var listPdf = new List<FileDto>();
        var listAllSignee = bulkSign.ListDoc.Select(doc =>
        {
            var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(doc.SignPositionDesc);
            var signee = new SignatureDto
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
                .Load(signee.UserIdentifier)
                .Build();

            signee.UserIdentifier = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;
            
            listPdf.Add(new FileDto
            {
                Filename = doc.UploadedDocId,
                Signatures = new List<SignatureDto>() { signee },
            });
            
            return signee;
        });

        var signatures = listAllSignee
            .GroupBy(s => s.UserIdentifier)
            .Select((signee, index) => new SignaturesDto
            {
                UserIdentifier = signee.First().UserIdentifier,
            }).ToList();

        return new RequestSignPayload
        {
            RequestId = bulkSign.BulkSignId,
            Signatures = signatures,
            ListPdf = listPdf,
        };
    }

    private List<BulkSignDocModel> UpdateAuthUrl(ReqBulkSignRequest req, RequestBulkSignResponseDto res)
    {
        return req.BulkSign.ListDoc.Select(doc =>
        {
            var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(doc.SignPositionDesc);
            var userIdentifierFromDesc = signPositionDescJson?.GetValueOrDefault("user_identifier").GetString() ?? string.Empty;
                    
            var tilakaUser = _tilakaUserBuilder
                .Load(userIdentifierFromDesc)
                .Build();
                    
            var userProvider = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;

            var authUrl = res.AuthUrls
                .FirstOrDefault(auth => auth.UserIdentifier == userProvider);

            if (authUrl is not null)
                doc.SignUrl = authUrl.Url;

            return doc;
        }).ToList();
    }
    
    private List<BulkSignDocModel> UpdateRequestBulkSignState(ReqBulkSignRequest req, RequestBulkSignResponseDto res)
    {
        res.FailedDocName.ForEach(failedDoc =>
        {
            var originalDoc = req.BulkSign.ListDoc.FirstOrDefault(x => x.UploadedDocId == failedDoc);
            if (originalDoc is not null)
                originalDoc.RequestBulkSignState = RequestBulkSignStateEnum.Failed;
        });

        return req.BulkSign.ListDoc;
    }
}