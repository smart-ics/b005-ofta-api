using System.Text.Json;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using Microsoft.Extensions.Options;
// ReSharper disable InconsistentNaming

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class SendToTekenAjaService : ISendToSignProviderService
{
    //  TODO: Implement SendToTekenAjaService
    //      kirim dokumen dengan nama sesuai request dan content-nya (base64)
    //      ke server TekenAJa. Lalu kembalikan response dari server TekenAJa
    //      yang berupa DocumentId (GUID) sebagai return value-nya
    private readonly TekenAjaProviderOptions _opt;

    public SendToTekenAjaService(IOptions<TekenAjaProviderOptions> options)
    {
        _opt = options.Value;
    }

    public SendToSignProviderResponse Execute(SendToSignProviderRequest req)
    {
        var data = Task.Run(() => GetDocIdTekenAja(req)).GetAwaiter().GetResult();
        var result = new SendToSignProviderResponse { UploadedDocId = data?.data.id ?? string.Empty };
        return result;
    }

    private async Task<ResponseTekenAjaDto?> GetDocIdTekenAja(SendToSignProviderRequest request)
    {
        //  BUILD REQUEST
        var filePathName = request.doc.RequestedDocUrl;
        var docPageCount = PdfHelper.GetPageCount(filePathName);
        var expiredDate = request.doc.DocDate.AddDays(_opt.ValidityPeriod).ToString("yyyy-MM-dd");
        var listSignee = request.doc.ListSignees
            .Select(x => new TekenAjaSignatureReqDto
            {
                email = x.Email,
                detail = _opt.SignLayout
                    .Where(y => y.SignPosition == (int)x.SignPosition)
                    .Select(y => new TekenAjaSignatureDetailReqDto
                {
                    p = docPageCount,
                    x = y.x,
                    y = y.y,
                    w = y.w,
                    h = y.h
                }).ToList()
            }).ToList();
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
        var resp = JsonSerializer.Deserialize<ResponseTekenAjaDto>(response.Content ?? string.Empty);


        //  RETURN
        return resp;
    }

    public class ResponseTekenAjaDto
    {
        public string status { get; set; }
        public string ref_id { get; set; }
        public string code { get; set; }
        public string timestamp { get; set; }
        public string message { get; set; }
        public RespTekenAjaDataDto data { get; set; }

    }
    public class RespTekenAjaDataDto
    {
        public string id { get; set; }
        public string[] stamp { get; set; }
        public List<RespoTekenAjaSignDto> sign
        {
            get; set;
        }

        public class RespoTekenAjaSignDto
        {
            public string teken_id { get; set; }
            public string email { get; set; }
            public string url { get; set; }
        }

    }

    public class TekenAjaSignatureDetailReqDto
    {
        public int p { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }

    public class TekenAjaSignatureReqDto
    {
        public string email { get; set; }
        public List<TekenAjaSignatureDetailReqDto> detail { get; set; }
    }
}