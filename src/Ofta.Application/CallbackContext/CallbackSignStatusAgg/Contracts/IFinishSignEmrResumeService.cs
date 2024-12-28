using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;

public record FinishSignEmrResumeRequest(string DocId);

public record FinishSignEmrResumeResponse(bool Success);
    
public interface IFinishSignEmrResumeService: INunaService<FinishSignEmrResumeResponse, FinishSignEmrResumeRequest>
{
}