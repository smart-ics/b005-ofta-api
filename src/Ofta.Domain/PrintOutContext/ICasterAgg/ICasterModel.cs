namespace Ofta.Domain.PrintOutContext.ICasterAgg;

public class ICasterModel 
{
    
    public ICasterModel()
    {
    }
    public string FromUser { get; set; }
    public string ToUser { get; set; }
    public MessageModel Message { get; set; }
}

public class MessageModel
{
    public string KlaimBpjsId { get; set; }
    public string PrintOutReffId { get; set; }
    public string Base64Content { get; set; }
    public string Type { get; set; }  
    public string RegId { get; set; }
}
