using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;


public record ReqSignToSignProviderRequest(DocModel Doc, string DocIdTilaka);

public class ReqSignToSignProviderResponse
{
    public List<DocSigneeModel> Signees { get; set; } = new List<DocSigneeModel>();
}
public interface IReqSignToSignProviderService
    : INunaService<ReqSignToSignProviderResponse, ReqSignToSignProviderRequest>
{
}