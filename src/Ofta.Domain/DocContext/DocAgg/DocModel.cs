using Ofta.Domain.UserContext;

namespace Ofta.Domain.DocContext.DocAgg;

public class DocModel : IDocKey, IUserKey
{
    public string DocId { get; set; }
    public DateTime DocDate { get; set; }
    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public DocStateEnum DocState { get; set; }
    public string RequestedDocUrl { get; set; }
    public string UploadedDocId { get; set; }
    public string UploadedDocUrl { get; set; }
    public string PublishedDocUrl { get; set; }
    
    public List<DocSigneeModel> ListSignees { get; set; }
    public List<DocJurnalModel> ListJurnal { get; set; }
}