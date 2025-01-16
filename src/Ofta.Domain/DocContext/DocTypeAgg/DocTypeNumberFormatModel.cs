namespace Ofta.Domain.DocContext.DocTypeAgg;

public class DocTypeNumberFormatModel: IDocTypeKey
{
    public DocTypeNumberFormatModel() { }
    public DocTypeNumberFormatModel(string id) => DocTypeId = id;
    public string DocTypeId { get; set; }
    public string Format { get; set; } = string.Empty;
    public ResetByEnum ResetBy { get; set; } = ResetByEnum.Month;
}

public enum ResetByEnum
{
    Day = 0,
    Month = 1,
    Year = 2,
    Continuous = 3
}