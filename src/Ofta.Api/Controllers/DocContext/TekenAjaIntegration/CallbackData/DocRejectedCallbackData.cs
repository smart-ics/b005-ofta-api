// ReSharper disable InconsistentNaming
namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CallbackData;

public class DocRejectedCallbackData
{
    public string document_id { get; set; }
    public string reject_reason { get; set; }
    public DocRejectedCallbackDataSignee signer_rejected { get; set; }
}

public class DocRejectedCallbackDataSignee
{
    public string email { get; set; }
    public string teken_id { get; set; }
}