namespace Ofta.Domain.DocContext.DocTypeAgg;

public class DocTypeNumberValueModel: IDocTypeKey
{
    public string DocTypeId { get; set; }
    public int Value { get; set; }
    public int PeriodeHari { get; set; }
    public int PeriodeBulan { get; set; }
    public int PeriodeTahun { get; set; }
}