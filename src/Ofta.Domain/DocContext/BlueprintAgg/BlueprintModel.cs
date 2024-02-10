using Ofta.Domain.DocContext.BundleSpecAgg;

namespace Ofta.Domain.DocContext.BlueprintAgg;

public class BlueprintModel : IBlueprintKey
{
    public string BlueprintId { get; set; }
    public string BlueprintName { get; set; }
    public List<BlueprintDocTypeModel> ListBlueprintDocType { get; set; }
}