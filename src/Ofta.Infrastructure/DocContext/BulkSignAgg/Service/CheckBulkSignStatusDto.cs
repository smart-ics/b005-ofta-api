using System.Text.Json.Serialization;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg.Service;

internal class CheckBulkSignStatusDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("status")]
    public List<StatusItem> Status { get; set; }
    
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }
    
    [JsonPropertyName("list_pdf")]
    public List<PdfItem> ListPdf { get; set; }
}

internal class StatusItem
{
    [JsonPropertyName("sequence")]
    public int Sequence { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("user_identifier")]
    public string UserIdentifier { get; set; }
    
    [JsonPropertyName("num_signatures")]
    public int NumSignatures { get; set; }
    
    [JsonPropertyName("num_signatures_done")]
    public int NumSignaturesDone { get; set; }
}

internal class PdfItem
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; }
    
    [JsonPropertyName("error")]
    public bool Error { get; set; }
    
    [JsonPropertyName("presigned_url")]
    public string PresignedUrl { get; set; }
}