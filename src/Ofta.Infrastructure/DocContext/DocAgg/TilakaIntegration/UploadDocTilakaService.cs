﻿using Microsoft.Extensions.Options;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;

public class UploadDocTilakaService : ISendToSignProviderService
{
    private readonly TilakaProviderOptions _opt;
    private readonly IParamSistemDal _paramSistemDal;
    private readonly ITokenTilakaService _token;

    public UploadDocTilakaService(IOptions<TilakaProviderOptions> options, IParamSistemDal paramSistemDal, ITokenTilakaService token)
    {
        _opt = options.Value;
        _paramSistemDal = paramSistemDal;
        _token = token;
    }

    public SendToSignProviderResponse Execute(SendToSignProviderRequest req)
    {
        var data = Task.Run(() => GetDocIdTilaka(req)).GetAwaiter().GetResult();
        var result = new SendToSignProviderResponse { UploadedDocId = data?.Filename ?? string.Empty };
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

        var options = new RestClientOptions(_opt.UploadEndpoint)
        {
            Authenticator = new JwtAuthenticator(token)
        };
        
        var client = new RestClient(options);
        var req = new RestRequest("/upload")
            .AddFile("file", filePathName);

        //  EXECUTE
        var response = await client.ExecutePostAsync(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var resp = JsonSerializer.Deserialize<UploadDocToTilakaResponse>(response.Content ?? string.Empty,jsonOptions);

        //  RETURN
        return resp;
    }
    
    private record UploadDocToTilakaResponse(string Succes, string Message, string Filename);
}
