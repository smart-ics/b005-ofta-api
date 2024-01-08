using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public record SendToSignProviderRequest(string FileName, string FielContentBase64);

public class SendToSignProviderResponse
{
    public string UploadedDocId { get; set; }
}
public interface ISendToSignProviderService : INunaService<SendToSignProviderResponse, SendToSignProviderRequest>
{
    
}