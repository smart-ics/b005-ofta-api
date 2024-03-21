using AutoMapper.Configuration.Conventions;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;
using Ofta.Infrastructure.ParamContext;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class WriteFileService : IWriteFileService
{
    private readonly IParamSistemDal _paramSistemDal;

    public WriteFileService(IParamSistemDal paramSistemDal)
    { _paramSistemDal = paramSistemDal; }

    public string Execute(WriteFileRequest req)
    {
        
        //  write file to disk from base64 string content
        //  replace url dengan lokasi path
        var paramStoragePath = _paramSistemDal.GetData(Sys.LocalStoragePath)
            ?? throw new KeyNotFoundException("Parameter StoragePath not found");
        var paramStorageUrl = _paramSistemDal.GetData(Sys.LocalStorageUrl)
            ?? throw new KeyNotFoundException("Parameter StorageUrl not found");
        var storagePath = req.FilePathName.Replace(paramStorageUrl.ParamSistemValue,paramStoragePath.ParamSistemValue);

        var fileBytes = Convert.FromBase64String(req.ContentBase64);
        File.WriteAllBytes(storagePath, fileBytes);
        return req.FilePathName;
    }
}