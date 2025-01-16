using System.Globalization;

namespace Ofta.Domain.DocContext.DocTypeAgg;

public class DocTypeModel : IDocTypeKey, IDocTypeFileUrl
{
    public DocTypeModel()
    {
    }

    public DocTypeModel(string id) => DocTypeId = id;
    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    public string DocTypeCode { get; set; }
    public bool IsStandard { get; set; }
    public bool IsActive { get; set; }
    public string FileUrl { get; set; }
    public string JenisDokRemoteCetak { get; set; }
    public string DefaultDrafterUserId { get; set; }
    public List<DocTypeTagModel> ListTag { get; set; }
    public DocTypeNumberFormatModel NumberFormat { get; set; }
}

public interface IDocTypeFileUrl : IDocTypeKey
{
    string FileUrl { get; }
}