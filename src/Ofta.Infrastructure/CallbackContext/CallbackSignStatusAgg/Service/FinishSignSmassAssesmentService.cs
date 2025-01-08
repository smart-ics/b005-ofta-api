using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;

namespace Ofta.Infrastructure.CallbackContext.CallbackSignStatusAgg.Service;

public class FinishSignSmassAssesmentService: IFinishSignSmassAssesmentService
{
    private readonly SmassOptions _opt;

    public FinishSignSmassAssesmentService(IOptions<SmassOptions> opt)
    {
        _opt = opt.Value;
    }
    
    public IFinishSignSmassAssesmentResponse Execute(IFinishSignSmassAssesmentRequest req)
    {
        var result = Task.Run(() => ExecuteAsync(req)).GetAwaiter().GetResult();
        return new IFinishSignSmassAssesmentResponse(result);
    }
    
    private async Task<bool> ExecuteAsync(IFinishSignSmassAssesmentRequest assesmentRequest)
    {
        //BUILD
        var endPoint = $"{_opt.BaseApiUrl}/api/Assesment/finishSign";
        var client = new RestClient(endPoint);
        var req = new RestRequest();
        
        var reqObj = new
        {
            oftaDocId = assesmentRequest.DocId, 
        };

        var reqBody = JsonSerializer.Serialize(reqObj);
        req.AddBody(reqBody, "application/json");

        //  EXECUTE
        var response = await client.ExecutePutAsync(req);

        //  RETURN 
        return response.StatusCode == HttpStatusCode.OK;
    }
}