﻿
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderSep : IReffIdFinderAction { }
public class ReffIdFinderSep : IReffIdFinderSep
{
    private readonly IGetSepService _getSepSvc;

    public ReffIdFinderSep(IGetSepService getSepSvc)
    {
        _getSepSvc = getSepSvc;
    }

    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result = _getSepSvc.Execute(regId) ?? new SepDto();
        return new List<string> { result.TrsSepId ?? string.Empty };
    }
}
