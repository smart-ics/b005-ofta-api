using Mapster;
using MassTransit;
using MediatR;
using MyHospital.MsgContract.Emr.MedisEvents;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.RegContext.RegAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases.Event;

public class ScanDocumentOnResumeMedisAdministratifCreatedNotifEvent: IConsumer<ResumeMedisAdministratifCreatedNotifEvent>
{
    private readonly IKlaimBpjsDal _klaimBpjsDal;
    private readonly IMediator _mediator;
    
    public ScanDocumentOnResumeMedisAdministratifCreatedNotifEvent(IKlaimBpjsDal klaimBpjsDal, IMediator mediator)
    {
        _klaimBpjsDal = klaimBpjsDal;
        _mediator = mediator;
    }

    public Task Consume(ConsumeContext<ResumeMedisAdministratifCreatedNotifEvent> context)
    {
        // CONSUME
        var regMsg = context.Message.Adapt<RegModel>();
        var klaimBpjs = _klaimBpjsDal.GetData(regMsg)
            ?? throw new KeyNotFoundException("KlaimBpjs not found");
        
        // WRITE
        var cmd = new KlaimBpjsPrintOutScanCommand(klaimBpjs.KlaimBpjsId);
        _mediator.Send(cmd);
        return Task.CompletedTask;
    }
}