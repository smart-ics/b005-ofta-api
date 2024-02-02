using Ofta.Application.DocContext.DocAgg.Contracts;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class SaveFileWorker : ISaveFileService
{
    public void Execute(SaveDocFileRequest req)
    {
        // Convert base64 string to byte array
        var fileBytes = Convert.FromBase64String(req.FileContentBase64);
        var filePathName = req.FilePathName;

        // Write the byte array to the file
        File.WriteAllBytesAsync(filePathName, fileBytes);
    }
}