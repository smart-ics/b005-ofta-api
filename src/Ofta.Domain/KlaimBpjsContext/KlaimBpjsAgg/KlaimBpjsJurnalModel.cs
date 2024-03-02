namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsJurnalModel
{
    public string KlaimBpjsId { get; set; }
    public int NoUrut { get; set; }
    public DateTime JurnalDate { get; set; }
    public string Description { get; set; }
    
    public KlaimBpjsStateEnum KlaimBpjsState { get; set; }
}