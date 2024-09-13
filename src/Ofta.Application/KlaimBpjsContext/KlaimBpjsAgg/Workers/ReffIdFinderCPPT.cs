namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderCppt : IReffIdFinderAction
{
}
public class ReffIdFinderCppt : IReffIdFinderCppt
{
    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result =  regId ?? string.Empty;
        return new List<string> { result };
    }
}