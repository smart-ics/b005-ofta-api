using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;


public interface IReffIdFinderAssesment : IReffIdFinderAction { }

public class ReffIdFinderAssesment : IReffIdFinderAssesment
{
    private readonly IListAssesmentService _service;

    public ReffIdFinderAssesment(IListAssesmentService service)
    {
        _service = service;
    }

    public IEnumerable<string> Find(string regId , string docTypeCode)
    {
        var result = _service.Execute(regId) ?? new List<AssesmentDto>();
        return result
                .Where(x => x.PaperId == docTypeCode)
                .Select(x => x.AssesmentId ?? string.Empty);
    }
}
