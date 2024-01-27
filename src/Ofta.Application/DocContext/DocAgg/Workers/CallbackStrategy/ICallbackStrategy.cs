namespace Ofta.Application.DocContext.DocAgg.Workers.CallbackStrategy;

public interface ICallbackStrategy
{
    void Handle(string callbackData);
}