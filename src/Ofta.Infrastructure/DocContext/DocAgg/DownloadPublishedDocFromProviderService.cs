using Ofta.Application.DocContext.DocAgg.Contracts;
using System.Net;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class DownloadPublishedDocFromProviderService : IDownloadPublishedDocFromProviderService
{
    //  TODO: Implement DownloadPublishedDocFromProviderService
    //      Download dokumen dari server TekenAJa dengan menggunakan DocumentId
    //      yang didapat dari response SendToSignProviderService
    //      Lalu simpan dokumen tersebut ke folder yang ditentukan
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