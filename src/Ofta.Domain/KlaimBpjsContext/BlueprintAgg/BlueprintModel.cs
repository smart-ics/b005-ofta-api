namespace Ofta.Domain.KlaimBpjsContext.BlueprintAgg;

public class BlueprintModel : IBlueprintKey
{
    public BlueprintModel(string blueprintId)
    {
        BlueprintId = blueprintId;
    }

    public BlueprintModel()
    {
    }
    public string BlueprintId { get; set; }
    public string BlueprintName { get; set; }
    public List<BlueprintDocTypeModel> ListDocType { get; set; }
}