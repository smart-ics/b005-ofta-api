
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderSpri : IReffIdFinderAction { }
public class ReffIdFinderSpri : IReffIdFinderSpri
{
    private readonly IListSkdpService _listSkdpSvc;

    public ReffIdFinderSpri(IListSkdpService listSkdpSvc)
    {
        _listSkdpSvc = listSkdpSvc;
    }
    public IEnumerable<string> Find(string regId)
    {
        var skdp = _listSkdpSvc.Execute(regId)?.ToList() ?? new List<SkdpDto>();
        var result = skdp.Where(x => x.IsSpri == true);
        return result.Select(x => x.NoSuratKontrol);
    }
}
