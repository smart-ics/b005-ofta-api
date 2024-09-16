using Mapster;
using Microsoft.Extensions.Options;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;


public class ListAssesmentService : IListAssesmentService
{
    private readonly SmassOptions _opt;

    public ListAssesmentService(IOptions<SmassOptions> opt)
    {
        _opt = opt.Value;
    }

    public IEnumerable<AssesmentDto> Execute(string req)
    {
        var listAssement = Task.Run(() => ListAssesment(req)).GetAwaiter().GetResult();
        if (listAssement is null) return null;

        var result = listAssement.Adapt<List<AssesmentDto>>();
        return result;
    }

    private async Task<IEnumerable<AssesmentDto>> ListAssesment(string regId)
    {
        //BUILD
        var endPoint = $"{_opt.BaseApiUrl}//api/Assesment/Catalog/{regId}";
        var client = new RestClient(endPoint);
        var req = new RestRequest();

        //  EXECUTE
        var response = await client.ExecuteGetAsync<JSend<IEnumerable<AssesmentDto>>>(req);

        //  RETURN 
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        return JSendResponse.Read(response);
    }
}

