namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsDocModel : IKlaimBpjsKey
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsId2 { get; set; }
    public int NoUrut { get; set; }

    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    
    public string DocId { get; set; }
    public string DocUrl { get; set; }
    
}
