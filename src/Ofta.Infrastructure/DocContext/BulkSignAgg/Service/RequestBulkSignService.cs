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
        var result = ExecuteAsync(req).GetAwaiter().GetResult();
        if (result?.AuthUrls is not null)
            req.BulkSign.ListDoc = UpdateAuthUrl(req, result);
        
        return new ReqBulkSignResponse(result?.Success == true, result?.Message ?? string.Empty, req.BulkSign);
    }

    private async Task<RequestBulkSignResponseDto?> ExecuteAsync(ReqBulkSignRequest request)
    {
        // BUILD
        var token = await _token.Execute(TilakaProviderOptions.SECTION_NAME);
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");
        
        var payload = GenerateRequestBulkSignPayload(request.BulkSign);
        var jsonPaylaod = JsonSerializer.Serialize(payload);
        
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
        var result = JsonSerializer.Deserialize<RequestBulkSignResponseDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }

    private RequestBulkSignPayload GenerateRequestBulkSignPayload(BulkSignModel bulkSign)
    {
        var listAllSignee = new List<SignatureEachDocDto>();
        var listPdf = new List<FileDto>();
        bulkSign.ListDoc.ForEach(doc =>
        {
            var listSigneeEachDoc = doc.ListSignee
                .Where(signee => !string.IsNullOrWhiteSpace(signee.SignPositionDesc))
                .Select(signee =>
                {
                    var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(signee.SignPositionDesc);
                    var newSignee = new SignatureEachDocDto
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

            
            listPdf.Add(new FileDto
            {
                Filename = doc.UploadedDocId,
                Signatures = listSigneeEachDoc,
            });
            
            listAllSignee.AddRange(listSigneeEachDoc);
        });

        var signatures = listAllSignee
            .GroupBy(s => s.UserIdentifier)
            .Select((signee, index) => new SignaturesDto
            {
                UserIdentifier = signee.First().UserIdentifier,
                SignatureImage = string.Empty,
                Sequence = index + 1,
            }).ToList();

        return new RequestBulkSignPayload
        {
            RequestId = bulkSign.BulkSignId,
            Signatures = signatures,
            ListPdf = listPdf,
        };
    }

    private List<BulkSignDocModel> UpdateAuthUrl(ReqBulkSignRequest req, RequestBulkSignResponseDto res)
    {
        req.BulkSign.ListDoc.ForEach(doc =>
        {
            doc.ListSignee = doc.ListSignee.Select(signee =>
            {
                var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(signee.SignPositionDesc);
                var userIdentifierFromDesc = signPositionDescJson?.GetValueOrDefault("user_identifier").GetString() ?? string.Empty;
                    
                var tilakaUser = _tilakaUserBuilder
                    .Load(userIdentifierFromDesc)
                    .Build();
                    
                var userProvider = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;

                var authUrl = res.AuthUrls
                    .FirstOrDefault(auth => auth.UserIdentifier == userProvider);

                if (authUrl is not null)
                    signee.SignUrl = authUrl.Url;

                return signee;
            }).ToList();
        });

        return req.BulkSign.ListDoc;
    }
}