using Nuna.Lib.DataTypeExtension;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderNotaBill : IReffIdFinderAction
{
}
public class ReffIdFinderNotaBill : IReffIdFinderNotaBill
{
    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result = regId ?? string.Empty;
        if (!string.IsNullOrEmpty(result))
        {
            result = "RO" + (result.Length >= 8 ? result.Substring(2, 8) : result.PadRight(8, '0'));
        }
        return new List<string> { result };
    }
}