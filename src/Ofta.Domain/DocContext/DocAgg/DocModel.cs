using System.Xml;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.DocContext.DocAgg;

public class DocModel : IDocKey, IUserOftaKey, IUploadedDocKey
{
    public DocModel()
    {
    }

    public DocModel(string id) => DocId = id;
    
    #region PROPS
    public string DocId { get; set; }
    public DateTime DocDate { get; set; }
    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public DocStateEnum DocState { get; set; }
    public string RequestedDocUrl { get; set; }
    public string UploadedDocId { get; set; }
    public string UploadedDocUrl { get; set; }
    public string PublishedDocUrl { get; set; }
    #endregion
    
    public List<DocSigneeModel> ListSignees { get; set; }
    public List<DocJurnalModel> ListJurnal { get; set; }
    public List<AbstractDocScopeModel> ListScope { get; set; }
}

public interface IUploadedDocKey
{
    string UploadedDocId { get; }
}