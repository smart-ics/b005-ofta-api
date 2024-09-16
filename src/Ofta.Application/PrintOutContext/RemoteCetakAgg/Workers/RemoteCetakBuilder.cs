using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Contracts;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;
using System.Globalization;

namespace Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;

public interface IRemoteCetakBuilder : INunaBuilder<RemoteCetakModel>
{
    IRemoteCetakBuilder Create(IRemoteCetakKey remoteCetakKey);
    IRemoteCetakBuilder Load(IRemoteCetakKey remoteCetakKey);
    IRemoteCetakBuilder Attach(RemoteCetakModel remoteCetak);
    IRemoteCetakBuilder JenisDoc(string jenisDoc);
    IRemoteCetakBuilder RemoteAddr(string remoteAddr);
    IRemoteCetakBuilder PrintState(int printState);
    IRemoteCetakBuilder Cetak(string tglCetak);
    IRemoteCetakBuilder JsonData(string jsonData);
    IRemoteCetakBuilder CallbackDataOfta(string callbackDataOfta);
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

    public IRemoteCetakBuilder Load(IRemoteCetakKey remoteCetakKey)
    {
        _agg = _remoteCetakDal.GetData(remoteCetakKey);
        return this;
    }
    public IRemoteCetakBuilder Create(IRemoteCetakKey remoteCetakKey)
    {
        _agg = new RemoteCetakModel
                {
                    KodeTrs = remoteCetakKey.KodeTrs,
                    TglSend = _tglJamDal.Now.ToString("yyyy-MM-dd"),
                    JamSend = _tglJamDal.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                    TglCetak = "3000-01-01",
                    JamCetak = "00:00:00",
                    PrintState  = 0
                };
        return this;
    }

    public IRemoteCetakBuilder Attach(RemoteCetakModel remoteCetak)
    {
        _agg = new RemoteCetakModel();
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

    public IRemoteCetakBuilder CallbackDataOfta(string callbackDataOfta)
    {
        _agg.CallbackDataOfta = callbackDataOfta;
        return this;
    }
}