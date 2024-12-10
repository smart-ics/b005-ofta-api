using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.CallbackContext.CallbackSignStatusAgg;

public class CallbackSignStatusDocModel: ICallbackSignStatusKey
{
    public string RequestId { get; set; }
    public string TilakaName { get; set; }
    public string UploadedDocId { get; set; }
    public string DownloadDocUrl { get; set; }
    public SignStateEnum UserSignState { get; set; }
}