using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.DocContext.DocAgg.Contracts;

public record DownloadPublishedDocFromProviderRequest(
    string DownloadUrl,
    string DestinationPathFileName);

public interface IDownloadPublishedDocFromProviderService : INunaServiceVoid<DownloadPublishedDocFromProviderRequest>
{
    
}