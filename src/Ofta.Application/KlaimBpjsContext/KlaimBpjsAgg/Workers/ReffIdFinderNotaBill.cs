﻿using Nuna.Lib.DataTypeExtension;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IReffIdFinderNotaBill : IReffIdFinderAction
{
}
public class ReffIdFinderNotaBill : IReffIdFinderNotaBill
{
    public IEnumerable<string> Find(string regId)
    {
        var result = regId ?? string.Empty;
        return new List<string> { result };
    }
}