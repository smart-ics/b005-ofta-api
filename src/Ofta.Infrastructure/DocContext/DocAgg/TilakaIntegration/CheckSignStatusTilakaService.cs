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
    private readonly IParamSistemDal _paramSistemDal;
    private readonly ITokenTilakaService _token;

    public CheckSignStatusTilakaService(IOptions<TilakaProviderOptions> options, IParamSistemDal paramSistemDal, ITokenTilakaService token)
    {
        _opt = options.Value;
        _paramSistemDal = paramSistemDal;
        _token = token;
    }

    public CheckSignStatusFromSignProviderResponse Execute(CheckSignStatusFromSignProviderRequest req)
    {
        var data = Task.Run(() => GetDownloadUrlTilaka(req)).GetAwaiter().GetResult();
        var result = new CheckSignStatusFromSignProviderResponse { DownloadUrl = data?.List_Pdf?.FirstOrDefault()?.Presigned_Url ?? string.Empty };
        return result;
    }

    #region Execute Sign
    private async Task<CheckSignStatusFromTilakaResponse?> GetDownloadUrlTilaka(CheckSignStatusFromSignProviderRequest request)
    {
        //  BUILD REQUEST
        var fileUrl = request.Doc.RequestedDocUrl;
        var token = await _token.Execute("TilakaProvider");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");

        //  replace url dengan lokasi path
        var paramStoragePath = _paramSistemDal.GetData(Sys.LocalStoragePath)
            ?? throw new KeyNotFoundException("Parameter StoragePath not found");
        var paramStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var filePathName = fileUrl.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue);

        var endpoint = _opt.UploadEndpoint + "/checksignstatus";
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);

        var jsonBody = new
        {
            request_id = request.Doc.DocId
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

        var resp = JsonSerializer.Deserialize<CheckSignStatusFromTilakaResponse>(response.Content ?? string.Empty, jsonOptions);

        //  RETURN
        return resp;
    }
    #endregion

    #region RESPONSE COMMAND
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
    #endregion
}
