﻿using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Infrastructure.DocContext.BulkSignAgg.Service;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;
public class ReqSignTilakaService : IReqSignToSignProviderService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _token;
    private readonly ITilakaUserBuilder _tilakaUserBuilder;

    public ReqSignTilakaService(IOptions<TilakaProviderOptions> options, ITokenTilakaService token, ITilakaUserBuilder tilakaUserBuilder)
    {
        _opt = options.Value;
        _token = token;
        _tilakaUserBuilder = tilakaUserBuilder;
    }

    public ReqSignToSignProviderResponse Execute(ReqSignToSignProviderRequest req)
    {
        var dataSign = Task.Run(() => GetUrlSignTilaka(req, req.DocIdTilaka)).GetAwaiter().GetResult();

        var result = new ReqSignToSignProviderResponse
        {
            Success = dataSign?.Success == true,
            Message = dataSign?.Message ?? string.Empty,
        };

        if (dataSign?.Auth_Urls != null)
        {
            result.Signees = req.Doc.ListSignees.Select(signee =>
            {
                var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(signee.SignPositionDesc);
                var userIdentifierFromDesc = signPositionDescJson?.GetValueOrDefault("user_identifier").GetString() ?? string.Empty;

                var tilakaUser = _tilakaUserBuilder
                   .Load(userIdentifierFromDesc)
                   .Build();

                var userProvider = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;

                var authUrl = dataSign.Auth_Urls
                    .FirstOrDefault(auth => auth.User_Identifier == userProvider);

                if (authUrl != null)
                {
                    signee.SignUrl = authUrl.Url;
                }

                return signee;
            }).ToList();
        }

        return result;
    }
    
    private async Task<ReqSignToTilakaResponse?> GetUrlSignTilaka(ReqSignToSignProviderRequest request, string docIdTilaka)
    {
        // BUILD REQUEST
        var token = await _token.Execute(TilakaProviderOptions.SECTION_NAME);
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        var requestId = request.Signee.DocSigneeId;
        
        var listSignee = request.Doc.ListSignees
            .Where(signee => !string.IsNullOrWhiteSpace(signee.SignPositionDesc) && signee.Email == request.Signee.Email)
            .Select(signee =>
            {
                var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(signee.SignPositionDesc);
                var reqSignee = new SignatureDto
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
                    .Load(reqSignee.UserIdentifier)
                    .Build();

                reqSignee.UserIdentifier = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;
                return reqSignee;
            }).ToList();
        
        var signatures = listSignee
            .GroupBy(s => s.UserIdentifier)
            .Select((signee, index) => new SignaturesDto
            {
                UserIdentifier = signee.First().UserIdentifier,
            }).ToList();
        
        var listPdf = new List<FileDto>()
        {
            new()
            {
                Filename = docIdTilaka,
                Signatures = listSignee
            }
        };

        var payload = new RequestSignPayload
        {
            RequestId = requestId,
            Signatures = signatures,
            ListPdf = listPdf,
        };

        var signJson = JsonSerializer.Serialize(payload);
        
        var options = new RestClientOptions(_opt.UploadEndpoint)
        {
            Authenticator = new JwtAuthenticator(token)
        };
        
        var client = new RestClient(options);
        var req = new RestRequest("/requestsign")
            .AddJsonBody(signJson);

        // EXECUTE
        var response = await client.ExecutePostAsync(req);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var respReqSign = JsonSerializer.Deserialize<ReqSignToTilakaResponse>(response.Content ?? string.Empty,jsonOptions);

        // RETURN
        return respReqSign;
    }
    
    private record ReqSignToTilakaResponse(bool Success, string Message, List<AuthUrlData> Auth_Urls);
    private record AuthUrlData(string Url, string User_Identifier);
}

