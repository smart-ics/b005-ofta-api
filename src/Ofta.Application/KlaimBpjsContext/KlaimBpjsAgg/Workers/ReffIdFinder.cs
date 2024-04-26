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
    private readonly IReffIdFinderResume _resumeFinder;
    private readonly IReffIdFinderNotaObat _notaObatFinder;
    private readonly IReffIdFinderHasilRadiologi _hasilRadiologiFinder;
    private readonly IReffIdFinderHasilLab _hasilLabFinder;
    private readonly IReffIdFinderSuratRujukan _suratRujukanFinder;
    private readonly IReffIdFinderSkdp _skdpFinder;
    private readonly IReffIdFinderSpri _spriFinder;
    private readonly IReffIdFinderSep _sepFinder;
    public ReffIdFinderFactory(IReffIdFinderTextEklaim textEklaim,
        IReffIdFinderNotaBill notaBill,
        IReffIdFinderResep resepFinder,
        IReffIdFinderResume resumeFinder,
        IReffIdFinderNotaObat notaObatFinder,
        IReffIdFinderHasilRadiologi hasilRadiologiFinder,
        IReffIdFinderHasilLab hasilLabFinder,
        IReffIdFinderSuratRujukan suratRujukanFinder,
        IReffIdFinderSkdp skdpFinder,
        IReffIdFinderSpri spriFinder,
        IReffIdFinderSep sepFinder)
    {
        _notaBillFinder = notaBill;
        _resepFinder = resepFinder;
        _textEklaimFinder = textEklaim;
        _resumeFinder = resumeFinder;
        _notaObatFinder = notaObatFinder;
        _hasilRadiologiFinder = hasilRadiologiFinder;
        _hasilLabFinder = hasilLabFinder;
        _suratRujukanFinder = suratRujukanFinder;
        _skdpFinder = skdpFinder;
        _spriFinder = spriFinder;
        _sepFinder = sepFinder;
    }

    public IReffIdFinderAction Factory(IKlaimBpjsKey klaimBpjsKey, IDocTypeKey docTypeKey)
        => docTypeKey.DocTypeId switch
        {
            "DTX01" => _textEklaimFinder,
            "DTX02" => _sepFinder,
            "DTX03" => _skdpFinder,
            "DTX04" => _spriFinder,
            "DTX05" => _resumeFinder,
            "DTX06" => _suratRujukanFinder,
            "DTX07" => _notaBillFinder,
            "DTX08" => _hasilRadiologiFinder,
            "DTX09" => _hasilLabFinder,
            "DTX0A" => new ReffIdFinderDefault(),
            "DTX0B" => new ReffIdFinderDefault(),
            "DTX0C" => _resepFinder,
            "DTX0D" => _notaObatFinder,
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

