
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using System.IO.Pipes;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderNotaObat : IReffIdFinderAction { }
public class ReffIdFinderNotaObat : IReffIdFinderNotaObat
{
    private readonly IListPenjualanService _listPenjualanSvc;

    public ReffIdFinderNotaObat(IListPenjualanService listPenjualanSvc)
    {
        _listPenjualanSvc = listPenjualanSvc;
    }

    public IEnumerable<string> Find(string regId)
    {
        var result = _listPenjualanSvc.Execute(regId);
        return result.Select(x => x.PenjualanId);
    }
}
