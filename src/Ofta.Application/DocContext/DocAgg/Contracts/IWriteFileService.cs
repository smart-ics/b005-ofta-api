using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public record WriteFileRequest(string FilePathName, string ContentBase64);

public interface IWriteFileService : INunaService<string, WriteFileRequest>
{
    
}