using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderSuratRujukan : IReffIdFinderAction { }
public class ReffIdFinderSuratRujukan : IReffIdFinderSuratRujukan
{
    private readonly IListRujukanFaskesService _listRujukanFaskesSvc;

    public ReffIdFinderSuratRujukan(IListRujukanFaskesService listRujukanFaskesSvc)
    {
        _listRujukanFaskesSvc = listRujukanFaskesSvc;
    }

    public IEnumerable<string> Find(string regId)
    {
        var result = _listRujukanFaskesSvc.Execute(regId);
        return result.Select(x => x.TrsRujukanFaskesId);
    }
}
