using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class CopyFileService : ICopyFileService
{
    private readonly IParamSistemDal _paramSistemDal;

    public CopyFileService(IParamSistemDal paramSistemDal)
    { _paramSistemDal = paramSistemDal; }

    public string Execute(CopyFileRequest req)
    {

        //  write file to disk from base64 string content
        //  replace url dengan lokasi path
        var paramStoragePath = _paramSistemDal.GetData(Sys.LocalStoragePath)
            ?? throw new KeyNotFoundException("Parameter StoragePath not found");
        var paramStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var storagePath = req.FilePathName.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue);
        var storageOriginPath = req.FilePathOriginName.Replace(paramStorageUrl.ParamSistemValue, paramStoragePath.ParamSistemValue);

        // Menyalin file dari storageOriginPath ke storagePath
        if (File.Exists(storageOriginPath))
        {
            File.Copy(storageOriginPath, storagePath, overwrite: true);
        }
        else
        {
            throw new FileNotFoundException($"File origin not found at {storageOriginPath}");
        }

        return req.FilePathName;
    }
}