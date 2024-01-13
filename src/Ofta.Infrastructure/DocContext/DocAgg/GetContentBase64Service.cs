using Ofta.Application.DocContext.DocAgg.Contracts;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class GetContentBase64Service : IGetContentBase64Service
{

    //  TODO: Implement GetContentBase64Service
    //      Open file, read content lalu ubah content tersebut jadi string base64
    //      dan kembalikan string base64 tersebut

    public string Execute(string filePathName)
    {
        File.ReadAllBytes(filePathName);
        string fileBase64 = Convert.ToBase64String(File.ReadAllBytes(filePathName));
        return fileBase64 ?? string.Empty;
    }
}