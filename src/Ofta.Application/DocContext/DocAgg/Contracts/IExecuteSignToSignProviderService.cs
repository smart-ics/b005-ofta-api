using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public record ExecuteSignToSignProviderRequest(DocModel Doc, IUserOftaKey UserOfta );

public class ExecuteSignToSignProviderResponse
{
    public string Status { get; set; }
    public string Message { get; set; }
}
public interface IExecuteSignToSignProviderService
    : INunaService<ExecuteSignToSignProviderResponse, ExecuteSignToSignProviderRequest>
{
}