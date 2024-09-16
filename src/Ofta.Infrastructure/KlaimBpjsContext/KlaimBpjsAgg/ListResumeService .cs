using Microsoft.Extensions.Options;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp.Authenticators;
using RestSharp;
using System.Net;
using Mapster;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListResumeService : IListResumeService
{
    private readonly Emr25Options _opt;

    public ListResumeService(IOptions<Emr25Options> opt)
    {
        _opt = opt.Value;
    }

    public IEnumerable<ResumeDto> Execute(string req)
    {
        var listResume = Task.Run(() => ListResume(req)).GetAwaiter().GetResult();
        if (listResume is null) return null;

        var result = listResume.Adapt<List<ResumeDto>>();
        return result;
    }

    private async Task<IEnumerable<ResumeDto>> ListResume(string regId)
    {
        //BUILD
        var endPoint = $"{_opt.BaseApiUrl}/api/ResumeMedis/List/{regId}/Register";
        var client = new RestClient(endPoint);
        var req = new RestRequest();

        //  EXECUTE
        var response = await client.ExecuteGetAsync<JSend<IEnumerable<ResumeDto>>>(req);

        //  RETURN 
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        return JSendResponse.Read(response);
    }
}
