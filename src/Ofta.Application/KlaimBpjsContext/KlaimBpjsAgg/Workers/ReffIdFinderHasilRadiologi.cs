
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using System.Security.AccessControl;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderHasilRadiologi : IReffIdFinderAction { }
public class ReffIdFinderHasilRadiologi : IReffIdFinderHasilRadiologi
{
    private readonly IListTindakanUmumService _listTdkUmumService;

    public ReffIdFinderHasilRadiologi(IListTindakanUmumService listTdkUmumService)
    {
        _listTdkUmumService = listTdkUmumService;
    }

    public IEnumerable<string> Find(string regId)
    {
        var listtdk = _listTdkUmumService.Execute(regId)?.ToList() 
            ?? new List<TindakanUmumDto>();
        var result = listtdk.Where(x => x.LayananTypeDkdId == "4");
        return result.Select(x => x.TindakanId ?? string.Empty);
    }
}
