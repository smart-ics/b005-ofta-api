// ReSharper disable InconsistentNaming
namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CallbackData;

public class DocSignCompletedCallbackData
{
    public string document_id { get; set; }
    public string document_file_name { get; set; }
    public string document_owner_name { get; set; }
    public string document_owner_email { get; set; }
    public string download_url { get; set; }
    public List<DocSignCompletedCallbackDataSigner> signers { get; set; }
    public List<object> stampers { get; set; }
}

public class DocSignCompletedCallbackDataSigner
{
    public string teken_id { get; set; }
    public string email { get; set; }
    public string name { get; set; }
}