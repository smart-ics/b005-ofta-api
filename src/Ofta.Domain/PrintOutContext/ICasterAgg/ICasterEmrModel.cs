namespace Ofta.Domain.PrintOutContext.ICasterAgg;

public class ICasterEmrModel
{
    public string FromUser { get; set; }
    public string ToUser { get; set; }
    public MessageEmrModel Message { get; set; }
}

public class MessageEmrModel
{
    public string DocType { get; set; }
    public string DocReff { get; set; }
}