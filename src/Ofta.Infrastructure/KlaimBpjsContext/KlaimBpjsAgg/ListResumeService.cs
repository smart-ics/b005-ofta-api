using System.Text.Json;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListResumeService: IListResumeService
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
        var endPoint = $"{_opt.BaseApiUrl}/api/ResumeMedis/List/{regId}/Register";
        var client = new RestClient(endPoint);
        var req = new RestRequest();

        //  EXECUTE
        var response = await client.ExecuteGetAsync<JSend<IEnumerable<ResumeRespDto>>>(req);

        //  RETURN 
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        
        var listResume = JSendResponse.Read(response);
        return listResume.IsNullOrEmpty() ? null : listResume;
    }
}