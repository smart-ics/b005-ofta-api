using Dawn;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.PrintOutContext.ICasterAgg;
using Ofta.Infrastructure.Helpers;
using RestSharp;


namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class SendToICasterService : ISendToICasterService
{
    private readonly ICasterOptions _opt;
    public SendToICasterService(IOptions<ICasterOptions> opt)
    {
        _opt = opt.Value;
    }
    public bool Execute(ICasterModel req)
    {
        Guard.Argument(() => req).NotNull();

        return ExecuteAsync(req).Result;
    }

    private async Task<bool> ExecuteAsync(ICasterModel req)
    {
        var endpoint = $"{_opt.BaseApiUrl}/api/Notif/toUser";
        var client = new RestClient(endpoint);

        var messageObj = new
        {
            klaimBpjsId = req.Message.KlaimBpjsId,
            printOutReffId = req.Message.PrintOutReffId,
            base64Content = req.Message.Base64Content,
            type = req.Message.Type,
            regId = req.Message.RegId
        };
        var messageJsonString = JsonConvert.SerializeObject(messageObj);

        var reqObj = new
        {
            fromUser = req.FromUser,
            toUser = req.ToUser,
            message = messageJsonString 
        };

        // Serialize objek anonim ke JSON string
        var reqBody = JsonConvert.SerializeObject(reqObj);
        var request = new RestRequest(endpoint, Method.Post)
            .AddBody(reqBody, "application/json");
        //  EXECUTE
        var response = await client.ExecuteAsync(request);

        //  RETURN
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    public bool Execute(ICasterEmrModel req)
    {
        Guard.Argument(() => req).NotNull();

        return ExecuteAsync(req).Result;
    }
    
    private async Task<bool> ExecuteAsync(ICasterEmrModel req)
    {
        var endpoint = $"{_opt.BaseApiUrl}/api/Notif/toUser";
        var client = new RestClient(endpoint);
        
        var reqObj = new
        {
            fromUser = req.FromUser,
            toUser = req.ToUser,
            message = req.Message 
        };
        
        var reqBody = JsonConvert.SerializeObject(reqObj);
        var request = new RestRequest(endpoint, Method.Post)
            .AddBody(reqBody, "application/json");
        
        //  EXECUTE
        var response = await client.ExecuteAsync(request);

        //  RETURN
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }
}
