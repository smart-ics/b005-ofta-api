namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class EmrNotificationModel
{
    public EmrNotificationModel(string userId, string message, string reffId)
    {
        UserrId = userId;
        Msg = message;
        ReffId = reffId;
        NotifTypeID = "N8";
    }
    
    public string UserrId { get; protected set; }
    public string Msg { get; protected set; }
    public string ReffId { get; protected set; }
    public string NotifTypeID { get; protected set; }
}