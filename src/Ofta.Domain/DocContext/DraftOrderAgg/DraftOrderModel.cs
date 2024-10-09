namespace Ofta.Domain.DocContext.DraftOrderAgg;

public class DraftOrderModel
{
    public string DraftOrderId { get; set; }
    public string DraftOrderDate { get; set; }

    public string RequesterUserId { get; set; }
    public string DrafterUserId { get; set; }

    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    public string Context { get; set; }
    public string ContextReffId { get; set; }
}

public interface IDraftOrderKey
{
    string DraftOrderId { get; }
}