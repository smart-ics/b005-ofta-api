using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;


public record IFinishSignSmassAssesmentRequest(string DocId);

public record IFinishSignSmassAssesmentResponse(bool Success);
    
public interface IFinishSignSmassAssesmentService: INunaService<IFinishSignSmassAssesmentResponse, IFinishSignSmassAssesmentRequest>
{
}