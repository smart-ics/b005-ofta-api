using Dawn;
using FluentValidation;
using Mapster;
using MassTransit;
using MediatR;
using MyHospital.MsgContract.Billing.AdmisiEvents;
using MyHospital.MsgContract.Billing.PaymentEvents;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;
using Ofta.Application.RegContext.RegAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.RegContext.RegAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;
using System.Threading;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.UseCases;

public record CreateOrderKlaimBpjsOnRegKeluarCreatedListenerDto(
    string RegId,
    string PasienId,
    string PasienName,
    string NoSep,
    string LayananName,
    string DokterName,
    string JenisInap,
    string UserOftaId = "ADMINOFTA") : IRegPasien, IUserOftaKey, IRegKey;
public class CreateOrderKlaimBpjsOnRegKeluarCreatedListener : IConsumer<RegOutCreatedNotifEvent>
{

    private OrderKlaimBpjsModel _agg = new();
    private readonly IOrderKlaimBpjsBuilder _builder;
    private readonly IOrderKlaimBpjsWriter _writer;
    private readonly IGetRegService _regSvc;
    private readonly IMediator _mediator;
    private readonly IValidator<CreateOrderKlaimBpjsOnRegKeluarCreatedListenerDto> _guard;

    public CreateOrderKlaimBpjsOnRegKeluarCreatedListener(IOrderKlaimBpjsBuilder builder,
                                    IOrderKlaimBpjsWriter writer,
                                    IValidator<CreateOrderKlaimBpjsOnRegKeluarCreatedListenerDto> guard,
                                    IMediator mediator, IGetRegService regSvc)
    {
        _builder = builder;
        _writer = writer;
        _regSvc = regSvc;
        _guard = guard;
        _mediator = mediator;
    }
    public Task Consume(ConsumeContext<RegOutCreatedNotifEvent> context)
    {
        // Konsume Message
        var regMsg = context.Message.Adapt<RegModel>();
        var regSvc = _regSvc.Execute(regMsg) ??
             throw new KeyNotFoundException($"register {regMsg.RegId} invalid");
        var reg = regSvc.Adapt<CreateOrderKlaimBpjsOnRegKeluarCreatedListenerDto>();
        //  GUARD
        var guardResult = _guard.Validate(reg);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);

        if (RegHasBeenIssued(reg))
            throw new ValidationException($"register {reg} is Issued");

        // BUILD
        _agg = _builder
            .Create()
            .Reg(reg)
            .Layanan(reg.LayananName)
            .Dokter(reg.DokterName)
            .RajalRanap(string.IsNullOrEmpty(reg.JenisInap.Trim()) ? RajalRanapEnum.Rajal : RajalRanapEnum.Ranap)
            .User(reg)
            .Build();

        //  WRITE
        _writer.Save(_agg);
        _mediator.Publish(new CreateOrderKlaimBpjsListenerEvent(_agg, reg));
        return Task.CompletedTask;

    }

    private bool RegHasBeenIssued(IRegKey RegId)
    {
        OrderKlaimBpjsModel? orderKlaimBpjs = null;
        var fallbackPolicy = Policy<bool>
            .Handle<KeyNotFoundException>()
            .Fallback(() => false);
        var isExist = fallbackPolicy.Execute(() =>
        {
            orderKlaimBpjs = _builder.LoadReg(RegId).Build();
            return !orderKlaimBpjs.OrderKlaimBpjsId.IsEmpty();
        });
        return isExist;
    }
}

public record CreateOrderKlaimBpjsListenerEvent(
    OrderKlaimBpjsModel Aggregate,
    CreateOrderKlaimBpjsOnRegKeluarCreatedListenerDto Command) : INotification;


public class CreateOrderKlaimBpjsOnRegKeluarCreatedListenerValidator : AbstractValidator<CreateOrderKlaimBpjsOnRegKeluarCreatedListenerDto>
{
    public CreateOrderKlaimBpjsOnRegKeluarCreatedListenerValidator()
    {
        RuleFor(x => x.RegId).NotEmpty();
        RuleFor(x => x.PasienId).NotEmpty();
        RuleFor(x => x.PasienName).NotEmpty();
        RuleFor(x => x.NoSep).NotEmpty();
        RuleFor(x => x.LayananName).NotEmpty();
        RuleFor(x => x.DokterName).NotEmpty();
    }
}