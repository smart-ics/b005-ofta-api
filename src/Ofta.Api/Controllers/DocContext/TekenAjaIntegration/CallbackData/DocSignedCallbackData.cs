// ReSharper disable InconsistentNaming
namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CallbackData;

public class DocSignedCallbackData
{
    public string document_id { get; set; }
    public string signer_teken_id { get; set; }
    public string signer_email { get; set; }
    public List<object> stamp { get; set; }
    public List<DocSignedCallbackDataSign> sign { get; set; }
}
public class DocSignedCallbackDataSign
{
    public string teken_id { get; set; }
    public string email { get; set; }
    public string url { get; set; }
}