namespace Ofta.Domain.ParamContext.SystemAgg;

public class ParamSistemModel : IParamSistemKey
{
    public ParamSistemModel()
    {
    }
    public ParamSistemModel(string paramSistemId)
    {
        ParamSistemId = paramSistemId;
    }
    public string ParamSistemId { get; set; }
    public string ParamSistemValue { get; set; }
}

public interface IParamSistemKey
{
    string ParamSistemId { get; }
}

public static class Sys
{
    public static IParamSistemKey LocalStoragePath => new ParamSistemModel("local-storage-path");
    public static IParamSistemKey OftaStorageUrl => new ParamSistemModel("ofta-storage-url");
}