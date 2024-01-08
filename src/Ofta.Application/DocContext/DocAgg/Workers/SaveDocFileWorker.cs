using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.DocContext.DocAgg.Workers;

public class SaveDocFileRequest
{
    public string FilePathName { get; set; }
    public string FileContentBase64 { get; set; }
}

public class SaveDocFileWorker : INunaServiceVoid<SaveDocFileRequest>
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