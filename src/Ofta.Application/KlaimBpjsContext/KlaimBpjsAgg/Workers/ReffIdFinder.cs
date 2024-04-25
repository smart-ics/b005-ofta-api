using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderAction
{
    IEnumerable<string> Find(string regId);
}

public interface IFactoryPattern<out TOut, in TIn1, in TIn2>
{
    TOut Factory(TIn1 arg1, TIn2 arg2);
}

public interface IReffIdFinderFactory : IFactoryPattern<IReffIdFinderAction, IKlaimBpjsKey, IDocTypeKey>
{
}

public class ReffIdFinderFactory : IReffIdFinderFactory
{
    private readonly IReffIdFinderTextEklaim _textEklaimFinder;
    private readonly IReffIdFinderNotaBill _notaBillFinder;
    private readonly IReffIdFinderResep _resepFinder;

    public ReffIdFinderFactory(IReffIdFinderTextEklaim textEklaim, 
        IReffIdFinderNotaBill notaBill, 
        IReffIdFinderResep resepFinder)
    {
        _notaBillFinder = notaBill;
        _resepFinder = resepFinder;
        _textEklaimFinder = textEklaim;
    }

    public IReffIdFinderAction Factory(IKlaimBpjsKey klaimBpjsKey, IDocTypeKey docTypeKey)
        => docTypeKey.DocTypeId switch
        {
            "DTX01" => _textEklaimFinder,
            "DTX02" => new ReffIdFinderDefault(),
            "DTX03" => new ReffIdFinderDefault(),
            "DTX04" => new ReffIdFinderDefault(),
            "DTX05" => new ReffIdFinderDefault(),
            "DTX06" => new ReffIdFinderDefault(),
            "DTX07" => _notaBillFinder,
            "DTX08" => new ReffIdFinderDefault(),
            "DTX09" => new ReffIdFinderDefault(),
            "DTX0A" => new ReffIdFinderDefault(),
            "DTX0B" => new ReffIdFinderDefault(),
            "DTX0C" => _resepFinder,
            "DTX0D" => new ReffIdFinderDefault(),
            "DTX0E" => new ReffIdFinderDefault(),
            _ => new ReffIdFinderDefault()
        };
}

public class ReffIdFinderDefault : IReffIdFinderAction
{
    public IEnumerable<string> Find(string regId)
    {
        return new List<string>();
    }
}

