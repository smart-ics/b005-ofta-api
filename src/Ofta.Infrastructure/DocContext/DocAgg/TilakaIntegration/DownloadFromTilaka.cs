using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;
using System.Net;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;

public class DownloadFromTilakaService : IDownloadPublishedDocFromProviderService
{
    private readonly IParamSistemDal _paramSistemDal;

    public DownloadFromTilakaService( IParamSistemDal paramSistemDal)
    {
        _paramSistemDal = paramSistemDal;
    }

    public void Execute(DownloadPublishedDocFromProviderRequest req)
    {
        string url = req.DownloadUrl;
        string fileUrl = req.DestinationPathFileName;

        //  replace url dengan lokasi path
        var paramStoragePath = _paramSistemDal.GetData(Sys.LocalStoragePath)
            ?? throw new KeyNotFoundException("Parameter StoragePath not found");
        var paramStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var filePathName = fileUrl.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue);

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(url, filePathName);
        }
    }
}