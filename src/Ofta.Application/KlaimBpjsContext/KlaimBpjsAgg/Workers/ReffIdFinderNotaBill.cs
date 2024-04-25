using Nuna.Lib.DataTypeExtension;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderNotaBill : IReffIdFinderAction
{
}
public class ReffIdFinderNotaBill : IReffIdFinderNotaBill
{
    public IEnumerable<string> Find(string regId)
    {
        if (regId.Length < 10)
            throw new ArgumentException($"RegID invalid: '{regId}'");
        
        var result = $"RO{regId.Right(8)}";

        return new List<string> { result };
    }
}