using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Contracts;

public record ExecuteBulkSignRequest(BulkSignModel BulkSign, string UserProvider );

public record ExecuteBulkSignResponse(bool Success, string Message);

public interface IExecuteBulkSignService
    : INunaService<ExecuteBulkSignResponse, ExecuteBulkSignRequest>
{
}