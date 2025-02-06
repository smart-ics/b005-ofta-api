using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderSbpk : IReffIdFinderAction {}

public class ReffIdFinderSbpk: IReffIdFinderSbpk
{
    private readonly IListResumeAdministratifService _resumeAdminService;
    private readonly IListResumeService _resumeService;

    public ReffIdFinderSbpk(IListResumeAdministratifService resumeAdminService, IListResumeService resumeService)
    {
        _resumeAdminService = resumeAdminService;
        _resumeService = resumeService;
    }

    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result = _resumeAdminService.Execute(regId)
             ??_resumeService.Execute(regId)
             ?? new List<ResumeDto>();
        
        return result.Select(x =>
        {
            if (!string.IsNullOrEmpty(x.ResumeId))
            {
                return x.ResumeId = x.ResumeId + "-" + docTypeCode;
            }
            return string.Empty;
        });
    }
}