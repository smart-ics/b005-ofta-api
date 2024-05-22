using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderResep : IReffIdFinderAction
{
}

public class ReffIdFinderResep : IReffIdFinderResep
{
    private readonly IListResepService _service;

    public ReffIdFinderResep(IListResepService service)
    {
        _service = service;
    }

    public IEnumerable<string> Find(string regId)
    {
        var result = _service.Execute(regId) ?? new List<ResepDto>();
        return result.Select(x => x.ResepId ?? string.Empty);
    }
}