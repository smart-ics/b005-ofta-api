using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsDocTypeModel : IKlaimBpjsKey, IDocTypeKey
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsDocTypeId { get; set; }
    public int NoUrut { get; set; }

    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    
    public List<KlaimBpjsPrintModel> ListPrint { get; set; }    
}