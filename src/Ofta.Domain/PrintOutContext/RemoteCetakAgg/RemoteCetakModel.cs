namespace Ofta.Domain.PrintOutContext.RemoteCetakAgg;

public class RemoteCetakModel : IRemoteCetakKey
{
    public RemoteCetakModel(string id) => KodeTrs = id;

    public RemoteCetakModel()
    {
    }
    public string KodeTrs {get;set;} 
    public string JenisDoc {get;set;}
    public string TglSend {get;set;} 
    public string JamSend {get;set;} 
    public string RemoteAddr {get;set;} 
    public int PrintState {get;set;}
    public string TglCetak {get;set;}
    public string JamCetak {get;set;}
    public string JsonData { get; set; }
}

public interface IRemoteCetakKey
{
    string KodeTrs { get; }
}
