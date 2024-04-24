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
    private readonly IReffIdFinderTextEklaim _reffIdFinderTextEklaim;

    public ReffIdFinderFactory(IReffIdFinderTextEklaim reffIdFinderTextEklaim)
    {
        _reffIdFinderTextEklaim = reffIdFinderTextEklaim;
    }

    public IReffIdFinderAction Factory(IKlaimBpjsKey klaimBpjsKey, IDocTypeKey docTypeKey)
        => docTypeKey.DocTypeId switch
        {
            "DTX01" => _reffIdFinderTextEklaim,
            // "DTX02" => new ReffIdFinderSep(),
            // "DTX03" => new ReffIdFinderSkdp(),
            // "DTX04" => new ReffIdFinderSpri(),
            // "DTX05" => new ReffIdFinderResumeMedis(),
            // "DTX06" => new ReffIdFinderSuratRujukan(),
            "DTX07" => new ReffIdFinderNotaBill(),
            // "DTX08" => new ReffIdFinderHasilRadiologi(),
            // "DTX09" => new ReffIdFinderHasilLaborat(),
            // "DTX0A" => new ReffIdFinderLaporanOperasi(),
            // "DTX0B" => new ReffIdFinderObservasiHd(),
            // "DTX0C" => new ReffIdFinderResep(),
            // "DTX0D" => new ReffIdFinderNotaObat(),
            // "DTX0E" => new ReffIdFinderCppt(),
            _ => throw new ArgumentOutOfRangeException()
        };
}

