
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderHasilLab : IReffIdFinderAction { }
public class ReffIdFinderHasilLab : IReffIdFinderHasilLab
{
    private readonly IListTindakanUmumService _listTdkUmumSvc;

    public ReffIdFinderHasilLab(IListTindakanUmumService listTdkUmumSvc)
    {
        _listTdkUmumSvc = listTdkUmumSvc;
    }

    public IEnumerable<string> Find(string regId)
    {
        var listtdk = _listTdkUmumSvc.Execute(regId)?.ToList()
            ?? new List<TindakanUmumDto>();
        var result = listtdk.Where(x => x.LayananTypeDkdId == "5");
        return result.Select(x => x.TindakanId);
    }
}
