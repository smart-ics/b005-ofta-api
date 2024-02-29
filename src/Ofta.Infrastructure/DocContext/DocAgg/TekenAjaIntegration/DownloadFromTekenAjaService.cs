using System.Net;
using Ofta.Application.DocContext.DocAgg.Contracts;

namespace Ofta.Infrastructure.DocContext.DocAgg.TekenAjaIntegration;

public class DownloadFromTekenAjaService : IDownloadPublishedDocFromProviderService
{
    //  TODO: Implement DownloadFromTekenAjaService
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