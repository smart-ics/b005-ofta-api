using Ofta.Application.DocContext.DocAgg.Contracts;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class SendToTekenAjaService : ISendToSignProviderService
{
    //  TODO: Implement SendToTekenAjaService
    //      kirim dokumen dengan nama sesuai request dan content-nya (base64)
    //      ke server TekenAJa. Lalu kembalikan response dari server TekenAJa
    //      yang berupa DocumentId (GUID) sebagai return value-nya
    public SendToSignProviderResponse Execute(SendToSignProviderRequest req)
    {
        return new SendToSignProviderResponse();
    }
}