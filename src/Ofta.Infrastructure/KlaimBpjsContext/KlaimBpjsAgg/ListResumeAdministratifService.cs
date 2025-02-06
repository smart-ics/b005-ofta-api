using Microsoft.Extensions.Options;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using Mapster;
using System.Text.Json;
using Nuna.Lib.ValidationHelper;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListResumeAdministratifService : IListResumeAdministratifService
{
    private readonly Emr25Options _opt;

    public ListResumeAdministratifService(IOptions<Emr25Options> opt)
    {
        _opt = opt.Value;
    }

    public IEnumerable<ResumeDto> Execute(string req)
    {
        var listResume = Task.Run(() => ListResume(req)).GetAwaiter().GetResult();
        if (listResume is null) return null;

        var result = listResume.Select(x => new ResumeDto
        {
            ResumeId = x.ResumeId,
            Tgljam = x.TglJam.ToDate("yyyy-MM-dd HH:mm:ss"),
            LayananId = x.LayananId,
            LayananName = x.LayananName,
            DokterId = x.DokterId,
            DokterName = x.DokterName
        }).ToList() ?? new List<ResumeDto>();
        return result;
    }

    private async Task<IEnumerable<ResumeRespDto>> ListResume(string regId)
    {
        //BUILD
        var endPoint = $"{_opt.BaseApiUrl}/api/ResumeMedis/ListAdministratif/{regId}/Register";
        var client = new RestClient(endPoint);
        var req = new RestRequest();

        //  EXECUTE
        var response = await client.ExecuteGetAsync<JSend<IEnumerable<ResumeRespDto>>>(req);

        //  RETURN 
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        var data = JsonSerializer.Serialize(response.Content);
        return JSendResponse.Read(response);
    }
}

public class ResumeRespDto
{
    public string ResumeId { get; set; }
    public string TglJam { get; set; }
    public string LayananId { get; set; }
    public string LayananName { get; set; }
    public string DokterId { get; set; }
    public string DokterName { get; set; }
}
