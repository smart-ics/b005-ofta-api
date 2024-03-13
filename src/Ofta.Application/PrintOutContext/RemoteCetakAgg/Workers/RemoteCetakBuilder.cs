using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Contracts;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;

namespace Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;

public interface IRemoteCetakBuilder : INunaBuilder<RemoteCetakModel>
{
    IRemoteCetakBuilder Create();
    IRemoteCetakBuilder Load(IRemoteCetakKey remoteCetakKey);
    IRemoteCetakBuilder Attach(RemoteCetakModel remoteCetak);
    IRemoteCetakBuilder KodeTrs(IRemoteCetakKey kodeTrsKey);
    IRemoteCetakBuilder JenisDoc(string jenisDoc);
    IRemoteCetakBuilder RemoteAddr(string remoteAddr);
    IRemoteCetakBuilder PrintState(int printState);
    IRemoteCetakBuilder Cetak(string tglCetak);
    IRemoteCetakBuilder JsonData(string jsonData);
}

public class RemoteCetakBuilder : IRemoteCetakBuilder
{
    private RemoteCetakModel _agg = new();
    private readonly IRemoteCetakDal _remoteCetakDal;
    private readonly ITglJamDal _tglJamDal;

    public RemoteCetakBuilder(IRemoteCetakDal remoteCetakDal, 
        ITglJamDal tglJamDal)
    {
        _remoteCetakDal = remoteCetakDal;
        _tglJamDal = tglJamDal;
    }

    public RemoteCetakModel Build()
    {
        _agg.RemoveNull();
        return _agg;
    }

    public IRemoteCetakBuilder Create()
    {
        var now = _tglJamDal.Now;
        _agg = new RemoteCetakModel
        {
            TglSend = now.ToString("yyyy-MM-dd"),
            JamSend = now.ToString("HH:mm:ss"),
        };
        return this;
    }

    public IRemoteCetakBuilder Load(IRemoteCetakKey remoteCetakKey)
    {
        _agg = _remoteCetakDal.GetData(remoteCetakKey)
            ?? throw new KeyNotFoundException("Data tidak ditemukan");
        return this;
    }

    public IRemoteCetakBuilder Attach(RemoteCetakModel remoteCetak)
    {
        _agg = new RemoteCetakModel();
        return this;
    }

    public IRemoteCetakBuilder KodeTrs(IRemoteCetakKey kodeTrsKey)
    {
        _agg.KodeTrs = kodeTrsKey.KodeTrs;
        return this;
    }

    public IRemoteCetakBuilder JenisDoc(string jenisDoc)
    {
        _agg.JenisDoc = jenisDoc;
        return this;
    }

    public IRemoteCetakBuilder RemoteAddr(string remoteAddr)
    {
        _agg.RemoteAddr = remoteAddr;
        return this;
    }

    public IRemoteCetakBuilder PrintState(int printState)
    {
        _agg.PrintState = printState;
        return this;
    }

    public IRemoteCetakBuilder Cetak(string tglCetak)
    {
        var now = _tglJamDal.Now;
        _agg.TglCetak = now.ToString("yyyy-MM-dd");
        _agg.JamCetak = now.ToString("HH:mm:ss");
        return this;
    }

    public IRemoteCetakBuilder JsonData(string jsonData)
    {
        _agg.JsonData = jsonData;
        return this;
    }
}