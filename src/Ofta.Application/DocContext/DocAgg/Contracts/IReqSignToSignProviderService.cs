using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;


// public record ReqSignToSignProviderRequest(DocModel Doc, string DocIdTilaka);
public record ReqSignToSignProviderRequest(DocModel Doc, DocSigneeModel Signee, string DocIdTilaka);

public class ReqSignToSignProviderResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<DocSigneeModel> Signees { get; set; } = new List<DocSigneeModel>();
}

public interface IReqSignToSignProviderService
    : INunaService<ReqSignToSignProviderResponse, ReqSignToSignProviderRequest>
{
}