using System.Net;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg.Service;

public class DownloadBulkSignFileService: IDownloadBulkSignFileService
{
    private readonly IParamSistemDal _paramSistemDal;

    public DownloadBulkSignFileService(IParamSistemDal paramSistemDal)
    {
        _paramSistemDal = paramSistemDal;
    }

    public void Execute(DownloadBulkSignFileRequest req)
    {
        var url = req.DownloadUrl;
        var fileUrl = req.DestinationPathFileName;

        //  replace url dengan lokasi path
        var paramStoragePath = _paramSistemDal.GetData(Sys.LocalStoragePath) 
            ?? throw new KeyNotFoundException("Parameter StoragePath not found");
        var paramStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var filePathName = fileUrl.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue);

        using var client = new WebClient();
        client.DownloadFile(url, filePathName);
    }
}