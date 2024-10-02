using Dawn;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;
using RestSharp;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class SendNotifToEmrService: ISendNotifToEmrService
{
    private readonly Emr20Options _opt;

    public SendNotifToEmrService(IOptions<Emr20Options> opt)
    {
        _opt = opt.Value;
    }

    public bool Execute(EmrNotificationModel req)
    {
        Guard.Argument(() => req).NotNull();

        return ExecuteAsync(req).Result;
    }

    private async Task<bool> ExecuteAsync(EmrNotificationModel req)
    {
        //BUILD
        var endPoint = $"{_opt.BaseApiUrl}/api/Notif/Add";
        var client = new RestClient(endPoint);
        var request = new RestRequest()
            .AddJsonBody(req);

        //  EXECUTE
        var response = await client.ExecutePostAsync(request);

        //  RETURN 
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }
}