using Ofta.Application.DocContext.DocAgg.Contracts;
using System.Net;

namespace Ofta.Infrastructure.DocContext.DocAgg.TilakaIntegration;

public class DownloadFromTilakaService : IDownloadPublishedDocFromProviderService
{
    public void Execute(DownloadPublishedDocFromProviderRequest req)
    {
        string url = req.DownloadUrl;
        string outputPath = req.DestinationPathFileName;

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(url, outputPath);
        }
    }
}