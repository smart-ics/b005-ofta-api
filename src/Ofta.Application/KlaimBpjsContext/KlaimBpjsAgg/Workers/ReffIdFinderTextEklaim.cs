namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderTextEklaim : IReffIdFinderAction
{
}
public class ReffIdFinderTextEklaim : IReffIdFinderTextEklaim
{
    public IEnumerable<string> Find(string regId)
    {
        return new[] { "text-eklaim-test" }.ToList();
    }
}
