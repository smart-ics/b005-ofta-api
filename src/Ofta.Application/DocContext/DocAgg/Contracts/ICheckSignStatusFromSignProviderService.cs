using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public record CheckSignStatusFromSignProviderRequest(
    DocModel Doc);

public class CheckSignStatusFromSignProviderResponse
{
    public string DownloadUrl { get; set; }
}
public interface ICheckSignStatusFromSignProviderService
    : INunaService<CheckSignStatusFromSignProviderResponse, CheckSignStatusFromSignProviderRequest>
{
}