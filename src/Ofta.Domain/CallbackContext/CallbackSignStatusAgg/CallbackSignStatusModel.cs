using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.CallbackContext.CallbackSignStatusAgg;

public class CallbackSignStatusModel: ICallbackSignStatusKey
{
    public CallbackSignStatusModel() {}
    public CallbackSignStatusModel(string requestId, string tilakaName)
    {
        RequestId = requestId;
        TilakaName = tilakaName;
    }
    public string RequestId { get; set; }
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public string TilakaName { get; set; }
    public DateTime CallbackDate { get; set; }
    public string JsonPayload { get; set; }
    public List<CallbackSignStatusDocModel> ListDoc { get; set; }

    public void SyncId() => ListDoc.ForEach(x =>
    {
        x.RequestId = RequestId;
        x.TilakaName = TilakaName;
    });
}