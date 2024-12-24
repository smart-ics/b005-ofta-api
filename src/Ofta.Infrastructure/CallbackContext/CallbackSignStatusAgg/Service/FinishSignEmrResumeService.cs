using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;

namespace Ofta.Infrastructure.CallbackContext.CallbackSignStatusAgg.Service;

public class FinishSignEmrResumeService: IFinishSignEmrResumeService
{
    private readonly Emr25Options _opt;

    public FinishSignEmrResumeService(IOptions<Emr25Options> opt)
    {
        _opt = opt.Value;
    }

    public FinishSignEmrResumeResponse Execute(FinishSignEmrResumeRequest req)
    {
        var result = Task.Run(() => ExecuteAsync(req)).GetAwaiter().GetResult();
        return new FinishSignEmrResumeResponse(result);
    }
    
    private async Task<bool> ExecuteAsync(FinishSignEmrResumeRequest resumeRequest)
    {
        //BUILD
        var endPoint = $"{_opt.BaseApiUrl}/api/ResumeMedis/finishSign";
        var client = new RestClient(endPoint);
        var req = new RestRequest();
        
        var reqObj = new
        {
            oftaDocId = resumeRequest.DocId, 
        };

        var reqBody = JsonSerializer.Serialize(reqObj);
        req.AddBody(reqBody, "application/json");

        //  EXECUTE
        var response = await client.ExecutePutAsync(req);

        //  RETURN 
        return response.StatusCode == HttpStatusCode.OK;
    }
}