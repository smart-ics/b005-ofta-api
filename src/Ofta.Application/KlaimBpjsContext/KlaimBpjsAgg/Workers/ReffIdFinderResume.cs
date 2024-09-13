
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderResume : IReffIdFinderAction { }

public class ReffIdFinderResume : IReffIdFinderResume
{
    private readonly IListResumeService _service;

    public ReffIdFinderResume(IListResumeService service)
    {
        _service = service;
    }

    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result = _service.Execute(regId) ?? new List<ResumeDto>();
        return result.Select(x => x.ResumeId ?? string.Empty);
    }
}
