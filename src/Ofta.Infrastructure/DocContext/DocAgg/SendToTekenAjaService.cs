using Dawn;
using MediatR;
using Newtonsoft.Json;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.UseCases;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class SendToTekenAjaService : ISendToSignProviderService
{
    //  TODO: Implement SendToTekenAjaService
    //      kirim dokumen dengan nama sesuai request dan content-nya (base64)
    //      ke server TekenAJa. Lalu kembalikan response dari server TekenAJa
    //      yang berupa DocumentId (GUID) sebagai return value-nya
    public SendToSignProviderResponse Execute(SendToSignProviderRequest req)
    {
        
        var data = Task.Run(() => GetDocIdTekenAja(req)).GetAwaiter().GetResult();
        var result = new SendToSignProviderResponse { UploadedDocId = data?.data.id ?? string.Empty };
        return result;
    }

    private async Task<ResponseTekenAjaDto?> GetDocIdTekenAja(SendToSignProviderRequest sign)
    {
        //  BUILD
        //convert content base64 ke file normal
        var fileBytes = Convert.FromBase64String(sign.FielContentBase64);
        string filePathName = $"c:/ics/damifile.pdf";

        // Write the byte array to the file
        _ = File.WriteAllBytesAsync(filePathName, fileBytes);

        string ExpiredDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");


        //TopLeft, TopCenter, TopRight,
        //BottomLeft, BottomCenter, BottomRight

        //[{ "email":"tio.030385@gmail.com",
        //"detail":[{ "p":1,"x":180,"y":245,"w":25,"h":25},
        //{ "p":2,"x":180,"y":245,"w":25,"h":25}]}]

        var DamiEmail = sign.FileName.ListSignees.FirstOrDefault() ?? new DocSigneeModel();
        var Email = DamiEmail.Email;

        var signature = sign.FileName.ListSignees.FirstOrDefault() ?? new DocSigneeModel();
        var posisi = signature.SignPosition;
        var level = signature.Level;

        var signature2 = sign.FileName.ListSignees.LastOrDefault() ?? new DocSigneeModel();
        var posisi2 = signature2.SignPosition;

        SignatureReq signatureReq = new SignatureReq();

        signatureReq.email = Email;

        SignatureReqItem signatureReqItem = new SignatureReqItem
        {
            p = 1,
            x = 180,
            y = 245,
            w = 25,
            h = 25
        };

        signatureReq.detil = new List<SignatureReqItem>
            {
                signatureReqItem
             };

        string SignJson = JsonConvert.SerializeObject(signatureReq);



        var endpoint = $"https://apix.sandbox-111094.com/v2/document/upload";
        var client = new RestClient(endpoint);
        var req = new RestRequest()
            .AddHeader("apikey", "cn8I4RBaFWB2bec9lfYDQVpDSUlt751K")
            .AddFile("document", filePathName)
            .AddParameter("signature", SignJson)
            .AddParameter("expiration_date", ExpiredDate)
            .AddParameter("is_in_order",level );
        req.Method = Method.Post;

        //  EXECUTE
        var response = await client.ExecuteAsync(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        var resp = JsonConvert.DeserializeObject<ResponseTekenAjaDto>(response.Content ?? string.Empty);


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
        public string stamp { get; set; }
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

    public class SignatureReq
    {
        public string email { get; set; }
        public List<SignatureReqItem> detil {get; set; }
    }

    public class SignatureReqItem
    {
        public int p { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}