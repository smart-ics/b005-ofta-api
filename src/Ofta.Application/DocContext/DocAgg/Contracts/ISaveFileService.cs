using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public class SaveDocFileRequest
{
    public string FilePathName { get; set; }
    public string FileContentBase64 { get; set; }
}
public interface ISaveFileService : INunaServiceVoid<SaveDocFileRequest>
{
    
}