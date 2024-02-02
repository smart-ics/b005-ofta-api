using System.Text.Json;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using Microsoft.Extensions.Options;
// ReSharper disable InconsistentNaming

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class SendToTekenAjaService : ISendToSignProviderService
{
    private readonly TekenAjaProviderOptions _opt;

    public SendToTekenAjaService(IOptions<TekenAjaProviderOptions> options)
    {
        _opt = options.Value;
    }

    public SendToSignProviderResponse Execute(SendToSignProviderRequest req)
    {
        var data = Task.Run(() => GetDocIdTekenAja(req)).GetAwaiter().GetResult();
        var result = new Application.DocContext.DocAgg.Contracts.SendToSignProviderResponse { UploadedDocId = data?.data.id ?? string.Empty };
        return result;
    }

    private async Task<SendToTekenAjaResponse?> GetDocIdTekenAja(SendToSignProviderRequest request)
    {
        //  BUILD REQUEST
        var filePathName = request.doc.RequestedDocUrl;
        var docPageCount = PdfHelper.GetPageCount(filePathName);
        var expiredDate = request.doc.DocDate.AddDays(_opt.ValidityPeriod).ToString("yyyy-MM-dd");
        var listSignee = request.doc.ListSignees
            .Select(x => new SendToTekenAjaRequest(x.Email, _opt.SignLayout
                .Where(y => y.SignPosition == (int)x.SignPosition)
                .Select(y => new SendToTekenAjaRequestDetail(docPageCount, y.x, y.y, y.w, y.h))));
        
        var signJson = JsonSerializer.Serialize(listSignee);
        var endpoint = _opt.UploadEnpoint;
        var client = new RestClient(endpoint);
        var req = new RestRequest()
            .AddHeader("apikey", _opt.ApiKey)
            .AddFile("document", filePathName)
            .AddParameter("signature", signJson)
            .AddParameter("expiration_date", expiredDate)
            .AddParameter("is_in_order","1" );
        req.Method = Method.Post;

        //  EXECUTE
        var response = await client.ExecuteAsync(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        var resp = JsonSerializer.Deserialize<SendToTekenAjaResponse>(response.Content ?? string.Empty);

        //  RETURN
        return resp;
    }

    private record SendToTekenAjaResponse(string status, string ref_id, string code, string timestamp, string message, SendToTekenAjaResponseData data);
    private record SendToTekenAjaResponseData(string id, string[] stamp, IEnumerable<SendToTekenAjaResponseDataSign> sign);
    private record SendToTekenAjaResponseDataSign(string teken_id, string email, string url);

    private record SendToTekenAjaRequest(string email, IEnumerable<SendToTekenAjaRequestDetail> detail);
    private record SendToTekenAjaRequestDetail(int p, int x, int y, int w, int h);
}