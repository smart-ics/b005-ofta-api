using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;
using Ofta.Infrastructure.Helpers;
using RestSharp.Authenticators;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;
public class ReqSignTilakaService : IReqSignToSignProviderService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _token;

    public ReqSignTilakaService(IOptions<TilakaProviderOptions> options, ITokenTilakaService token)
    {
        _opt = options.Value;
        _token = token;
    }

    public ReqSignToSignProviderResponse Execute(ReqSignToSignProviderRequest req)
    {
        var dataSign = Task.Run(() => GetUrlSignTilaka(req, req.DocIdTilaka)).GetAwaiter().GetResult();

        var result = new ReqSignToSignProviderResponse();

        if (dataSign?.auth_urls != null)
        {
            result.Signees = req.doc.ListSignees.Select(signee =>
            {
                var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(signee.SignPositionDesc);
                var userIdentifierFromDesc = signPositionDescJson.GetValueOrDefault("user_identifier").GetString();

                var authUrl = dataSign.auth_urls
                    .FirstOrDefault(auth => auth.user_identifier == userIdentifierFromDesc);

                if (authUrl != null)
                {
                    signee.SignUrl = authUrl.url;
                }

                return signee;
            }).ToList();
        }

        return result;
    }


    #region REQUEST SIGN 
    private async Task<ReqSignToTilakaResponse?> GetUrlSignTilaka(ReqSignToSignProviderRequest request, string DocIdTilaka)
    {
        // BUILD REQUEST
        var token = await _token.Execute("TilakaProvider");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        var requestId = request.doc.DocId;

        var listSignee = request.doc.ListSignees
        .Where(signee => !string.IsNullOrWhiteSpace(signee.SignPositionDesc))
        .Select(signee =>
        {
            var signPositionDescJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(signee.SignPositionDesc);

            var userIdentifier = signPositionDescJson.GetValueOrDefault("user_identifier").GetString();
            var width = signPositionDescJson.GetValueOrDefault("width").GetInt32();
            var height = signPositionDescJson.GetValueOrDefault("height").GetInt32();
            var coordinateX = signPositionDescJson.GetValueOrDefault("coordinate_x").GetInt32();
            var coordinateY = signPositionDescJson.GetValueOrDefault("coordinate_y").GetInt32();
            var pageNumber = signPositionDescJson.GetValueOrDefault("page_number").GetInt32();
            var qrOption = signPositionDescJson.GetValueOrDefault("qr_option").GetString();

            return new
            {
                user_identifier = userIdentifier,
                width = width,
                height = height,
                coordinate_x = coordinateX,
                coordinate_y = coordinateY,
                page_number = pageNumber,
                qr_option = qrOption
            };
        }).ToList();


        int index = 0;
        var payload = new
        {
            request_id = requestId,
            signatures = listSignee
            .GroupBy(p => p.user_identifier)
            .Select((group, index) => new
            {
                user_identifier = group.First().user_identifier,
                signature_base64 = "",
                sequence = index + 1
            }).ToList(),
            list_pdf = new List<object>
        {
            new
            {
                filename = DocIdTilaka,
                signatures = listSignee.Select(signee => new
                {
                    signee.user_identifier,
                    signee.width,
                    signee.height,
                    signee.coordinate_x,
                    signee.coordinate_y,
                    signee.page_number,
                    signee.qr_option
                }).ToList()
            }
        }
        };

        var signJson = JsonSerializer.Serialize(payload);

        var endpoint = _opt.UploadEndpoint + "/requestsign";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);
        var req = new RestRequest()
            .AddJsonBody(signJson);

        req.Method = Method.Post;

        // EXECUTE
        var response = await client.ExecuteAsync(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;

        var respReqSign = JsonSerializer.Deserialize<ReqSignToTilakaResponse>(response.Content ?? string.Empty);

        // RETURN
        return respReqSign;
    }
    #endregion

    #region RESPONSE COMMAND
    private record ReqSignToTilakaResponse(string status, string message, List<AuthUrlData> auth_urls);
    private record AuthUrlData(string url, string user_identifier);
    #endregion
}

