using MediatR;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Workers;
using Ofta.Application.Helpers;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;

public record ReceiveCallbackSignStatusCommand(dynamic Data): IRequest;

public class ReceiveCallbackSignStatusHandler: IRequestHandler<ReceiveCallbackSignStatusCommand>
{
    private readonly ITglJamDal _tglJamDal;
    private readonly ICallbackSignStatusWriter _writer;

    public ReceiveCallbackSignStatusHandler(ITglJamDal tglJamDal, ICallbackSignStatusWriter writer)
    {
        _tglJamDal = tglJamDal;
        _writer = writer;
    }

    public Task<Unit> Handle(ReceiveCallbackSignStatusCommand request, CancellationToken cancellationToken)
    {
        var agg = new CallbackSignStatusModel
        {
            CallbackDate = _tglJamDal.Now,
            Payload = request.Data.ToString(),
        };

        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}