namespace Ofta.Domain.PrintOutContext.ICasterAgg;

public class ICasterEmrModel
{
    public ICasterEmrModel(string from, string to)
    {
        FromUser = from;
        ToUser = to;
        Message = "New Notification";
    }
    
    public string FromUser { get; set; }
    public string ToUser { get; set; }
    public string Message { get; set; }
}