namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsEventModel
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsJurnalId { get; set; }
    public int NoUrut { get; set; }
    public DateTime EventDate { get; set; }
    public string Description { get; set; }
}