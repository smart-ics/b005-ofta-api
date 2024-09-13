using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderTextEklaim : IReffIdFinderAction
{
}
public class ReffIdFinderTextEklaim : IReffIdFinderTextEklaim
{
    private readonly IGetSepService _getSepSvc;

    public ReffIdFinderTextEklaim(IGetSepService getSepSvc)
    {
        _getSepSvc = getSepSvc;
    }

    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result = _getSepSvc.Execute(regId) ?? new SepDto();
        return new List<string> { result.NoSep ?? string.Empty };
    }
}
