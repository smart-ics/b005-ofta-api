using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

public class BlueprintDocTypeModel : IBlueprintKey, IDocTypeKey
{
    public string BlueprintId { get; set; }
    public string BlueprintDocTypeId { get; set; }
    public int NoUrut { get; set; }
    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    public bool ToBePrinted { get; set; }
    
    public List<BlueprintSigneeModel> ListSignee { get; set; }
}