using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Contracts;

public record DownloadBulkSignFileRequest(
    string DownloadUrl,
    string DestinationPathFileName);
    
public interface IDownloadBulkSignFileService: INunaServiceVoid<DownloadBulkSignFileRequest>
{
}