using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public record SendToSignProviderRequest(DocModel FileName, string FielContentBase64);

public class SendToSignProviderResponse
{
    public string UploadedDocId { get; set; }
}
public interface ISendToSignProviderService : INunaService<SendToSignProviderResponse, SendToSignProviderRequest>
{
    
}