using Ofta.Application.DocContext.DocAgg.Contracts;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class WriteFileService : IWriteFileService
{
    public string Execute(WriteFileRequest req)
    {
        //  write file to disk from base64 string content
        var fileBytes = Convert.FromBase64String(req.ContentBase64);
        File.WriteAllBytes(req.FilePathName, fileBytes);
        return req.FilePathName;
    }
}