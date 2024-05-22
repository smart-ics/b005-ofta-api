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

    public IEnumerable<string> Find(string regId)
    {
        var result = _getSepSvc.Execute(regId);
        return new List<string> { result.TrsSepId };
    }
}
