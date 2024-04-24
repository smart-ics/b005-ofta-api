namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderNotaBill : IReffIdFinderAction
{
}
public class ReffIdFinderNotaBill : IReffIdFinderNotaBill
{
    public IEnumerable<string> Find(string regId)
    {
        throw new NotImplementedException();
    }
}