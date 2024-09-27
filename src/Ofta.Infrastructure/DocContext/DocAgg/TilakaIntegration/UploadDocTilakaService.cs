using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;

public class UploadDocTilakaService : ISendToSignProviderService
{
    private readonly TilakaProviderOptions _opt;
    private readonly IParamSistemDal _paramSistemDal;
    private readonly ITokenService _token;

    public UploadDocTilakaService(IOptions<TilakaProviderOptions> options, IParamSistemDal paramSistemDal, ITokenService token)
    {
        _opt = options.Value;
        _paramSistemDal = paramSistemDal;
        _token = token;
    }

    public SendToSignProviderResponse Execute(SendToSignProviderRequest req)
    {
        var data = Task.Run(() => GetDocIdTilaka(req)).GetAwaiter().GetResult();
        var result = new SendToSignProviderResponse { UploadedDocId = data?.filename ?? string.Empty };
        return result;
    }

    private async Task<UploadDocToTilakaResponse?> GetDocIdTilaka(SendToSignProviderRequest request)
    {
        //  BUILD REQUEST
        var fileUrl = request.doc.RequestedDocUrl;
        var token = await _token.Execute("TilakaProvider");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.TokenEndPoint} failed");


        //  replace url dengan lokasi path
        var paramStoragePath = _paramSistemDal.GetData(Sys.LocalStoragePath)
            ?? throw new KeyNotFoundException("Parameter StoragePath not found");
        var paramStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var filePathName = fileUrl.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue);


        var docPageCount = PdfHelper.GetPageCount(filePathName);
        var endpoint = _opt.UploadEndpoint;
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);
        var req = new RestRequest()
            .AddFile("file", filePathName);

        req.Method = Method.Post;

        //  EXECUTE
        var response = await client.ExecuteAsync(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        var resp = JsonSerializer.Deserialize<UploadDocToTilakaResponse>(response.Content ?? string.Empty);

        //  RETURN
        return resp;
    }

    #region RESPONSE COMMAND
    private record UploadDocToTilakaResponse(string status, string message, string filename);
    #endregion
}
