namespace Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

public class BlueprintModel : IBlueprintKey
{
    public string BlueprintId { get; set; }
    public string BlueprintName { get; set; }
    public List<BlueprintDocTypeModel> ListDocType { get; set; }
}