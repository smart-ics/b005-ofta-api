using System.Text.Json.Serialization;

namespace Ofta.Infrastructure.DocContext.BulkSignAgg.Service;

internal class RequestSignPayload
{
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }
    
    [JsonPropertyName("signatures")]
    public List<SignaturesDto> Signatures { get; set; }
    
    [JsonPropertyName("list_pdf")]
    public List<FileDto> ListPdf { get; set; }
};

internal class FileDto
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; }
    
    [JsonPropertyName("signatures")]
    public List<SignatureDto> Signatures { get; set; }
};

internal class SignaturesDto
{
    [JsonPropertyName("user_identifier")]
    public string UserIdentifier { get; set; }
}

internal class SignatureDto
{
    [JsonPropertyName("user_identifier")]
    public string UserIdentifier { get; set; }
        
    [JsonPropertyName("reason")]
    public string Reason { get; set; }
        
    [JsonPropertyName("location")]
    public string Location { get; set; }
        
    [JsonPropertyName("width")]
    public double Width { get; set; }
        
    [JsonPropertyName("height")]
    public double Height { get; set; }
            
    [JsonPropertyName("coordinate_x")]
    public double CoordinateX { get; set; }
        
    [JsonPropertyName("coordinate_y")]
    public double CoordinateY { get; set; }
        
    [JsonPropertyName("page_number")]
    public int PageNumber { get; set; }
        
    [JsonPropertyName("qr_option")]
    public string QrOption { get; set; }
}
    
internal class RequestBulkSignResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
        
    [JsonPropertyName("message")]
    public string Message { get; set; }
        
    [JsonPropertyName("auth_urls")]
    public List<AuthUrlDto> AuthUrls { get; set; }
        
    [JsonPropertyName("failed_doc_name")]
    public List<string> FailedDocName { get; set; }
}

internal class AuthUrlDto
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
        
    [JsonPropertyName("user_identifier")]
    public string UserIdentifier { get; set; }
}