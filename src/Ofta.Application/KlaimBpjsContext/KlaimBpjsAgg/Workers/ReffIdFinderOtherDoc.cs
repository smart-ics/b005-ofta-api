namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderOtherDoc : IReffIdFinderAction
{
}
public class ReffIdFinderOtherDoc : IReffIdFinderOtherDoc
{
    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result = regId ?? string.Empty;
        if (!string.IsNullOrEmpty(result))
        {
            result = result + "-" + docTypeCode;
        }
        return new List<string> { result };
    }
}