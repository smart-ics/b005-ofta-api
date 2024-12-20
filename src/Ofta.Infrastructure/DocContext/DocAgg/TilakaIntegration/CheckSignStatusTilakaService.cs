using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp.Authenticators;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;

public class CheckSignStatusTilakaService : ICheckSignStatusFromSignProviderService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _token;

    public CheckSignStatusTilakaService(IOptions<TilakaProviderOptions> options, ITokenTilakaService token)
    {
        _opt = options.Value;
        _token = token;
    }

    public CheckSignStatusFromSignProviderResponse Execute(CheckSignStatusFromSignProviderRequest req)
    {
        var data = Task.Run(() => GetDownloadUrlTilaka(req)).GetAwaiter().GetResult();
        var result = new CheckSignStatusFromSignProviderResponse { DownloadUrl = data?.List_Pdf?.FirstOrDefault()?.Presigned_Url ?? string.Empty };
        return result;
    }
    
    private async Task<CheckSignStatusFromTilakaResponse?> GetDownloadUrlTilaka(CheckSignStatusFromSignProviderRequest request)
    {
        //  BUILD REQUEST
        var token = await _token.Execute("TilakaProvider");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        var payload = new
        {
            request_id = request.Doc.DocId
        };
        var signJson = JsonSerializer.Serialize(payload);
        
        var options = new RestClientOptions(_opt.BaseApiUrl)
        {
            Authenticator = new JwtAuthenticator(token)
        };

        var client = new RestClient(options);
        var req = new RestRequest("/checksignstatus")
            .AddJsonBody(signJson);

        req.Method = Method.Post;

        //  EXECUTE
        var response = await client.ExecuteAsync(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var resp = JsonSerializer.Deserialize<CheckSignStatusFromTilakaResponse>(response.Content ?? string.Empty, jsonOptions);

        //  RETURN
        return resp;
    }
    
    private record CheckSignStatusFromTilakaResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<StatusItem> Status { get; set; }
        public string Request_Id { get; set; }
        public List<PdfItem> List_Pdf { get; set; }

        public class StatusItem
        {
            public int Sequence { get; set; }
            public string Status { get; set; }
            public string User_Identifier { get; set; }
            public int Num_Signatures { get; set; }
            public int Num_Signatures_Done { get; set; }
        }

        public class PdfItem
        {
            public string Filename { get; set; }
            public bool Error { get; set; }
            public string Presigned_Url { get; set; }
        }
    }
}
