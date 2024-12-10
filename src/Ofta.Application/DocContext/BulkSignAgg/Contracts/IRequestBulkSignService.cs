using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Contracts;

public record ReqBulkSignRequest(BulkSignModel BulkSign);

public record ReqBulkSignResponse(bool Success, string Message, BulkSignModel BulkSign);

public interface IRequestBulkSignService: INunaService<ReqBulkSignResponse, ReqBulkSignRequest>
{
}