using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Contracts;

public record CheckBulkSignStatusRequest(BulkSignModel BulkSign);

public record FileItemResponse(string Filename, string DownloadUrl);

public record CheckBulkSignStatusResponse(bool Success, string Message, List<FileItemResponse> ListFiles);
    
public interface ICheckBulkSignStatusService: INunaService<CheckBulkSignStatusResponse, CheckBulkSignStatusRequest>
{
}