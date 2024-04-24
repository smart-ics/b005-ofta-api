namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsPrintOutModel
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsDocTypeId { get; set; }
    public string KlaimBpjsPrintOutId { get; set; }
    public int NoUrut { get; set; }
    public string DocId { get; set; }
    public string DocUrl { get; set; }
    public string PrintOutReffId { get; set; }
    public PrintStateEnum PrintState { get; set; }
    public List<KlaimBpjsSigneeModel> ListSign { get; set; }
}