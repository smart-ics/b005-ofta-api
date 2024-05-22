
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderSkdp : IReffIdFinderAction { }

public class ReffIdFinderSkdp : IReffIdFinderSkdp
{
    private readonly IListSkdpService _listSkdpSvc;

    public ReffIdFinderSkdp(IListSkdpService listSkdpSvc)
    {
        _listSkdpSvc = listSkdpSvc;
    }

    public IEnumerable<string> Find(string regId)
    {
        var skdp = _listSkdpSvc.Execute(regId)?.ToList() ?? new List<SkdpDto>();
        var result = skdp.Where(x => x.IsSpri == false);
        return result.Select(x => x.NoSuratKontrol ?? string.Empty);
    }
}
