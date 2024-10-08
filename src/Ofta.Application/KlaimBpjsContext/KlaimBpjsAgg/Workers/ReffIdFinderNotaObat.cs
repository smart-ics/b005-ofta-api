﻿
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using System.IO.Pipes;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderNotaObat : IReffIdFinderAction { }
public class ReffIdFinderNotaObat : IReffIdFinderNotaObat
{
    private readonly IListPenjualanService _listPenjualanSvc;

    public ReffIdFinderNotaObat(IListPenjualanService listPenjualanSvc)
    {
        _listPenjualanSvc = listPenjualanSvc;
    }

    public IEnumerable<string> Find(string regId, string docTypeCode)
    {
        var result = _listPenjualanSvc.Execute(regId) ?? new List<PenjualanDto>();
        return result.Select(x => x.PenjualanId ?? string.Empty);
    }
}
