using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.UseCases;

public record CreateOrderKlaimBpjsCommand(string UserOftaId, 
    string RegId, string PasienId, string PasienName, 
    string NoSep, string LayananName, string DokterName, 
    int RajalRanap) : IRequest<CreateOrderKlaimBpjsResponse>, IUserOftaKey, IRegPasien;

public record CreateOrderKlaimBpjsResponse(string OrderKlaimBpjsId);

public class CreateOrderKlaimBpjsHandler : IRequestHandler<CreateOrderKlaimBpjsCommand, CreateOrderKlaimBpjsResponse>
{
    private readonly IOrderKlaimBpjsBuilder _builder;
    private readonly IOrderKlaimBpjsWriter _writer;
    private readonly IValidator<CreateOrderKlaimBpjsCommand> _guard;

    public CreateOrderKlaimBpjsHandler(IOrderKlaimBpjsBuilder builder, 
        IOrderKlaimBpjsWriter writer, 
        IValidator<CreateOrderKlaimBpjsCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<CreateOrderKlaimBpjsResponse> Handle(CreateOrderKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILDER
        var order = _builder
            .Create()
            .Reg(request)
            .Layanan(request.LayananName)
            .Dokter(request.DokterName)
            .RajalRanap((RajalRanapEnum)request.RajalRanap)
            .User(request)
            .Build();

        //  WRITE
        _writer.Save(order);
        return Task.FromResult(new CreateOrderKlaimBpjsResponse(order.OrderKlaimBpjsId));
    }
}

public class CreateOrderKlaimBpjsValidator : AbstractValidator<CreateOrderKlaimBpjsCommand>
{
    public CreateOrderKlaimBpjsValidator()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.RegId).NotEmpty();
        RuleFor(x => x.PasienId).NotEmpty();
        RuleFor(x => x.PasienName).NotEmpty();
        RuleFor(x => x.NoSep).NotEmpty();
        RuleFor(x => x.LayananName).NotEmpty();
        RuleFor(x => x.DokterName).NotEmpty();
        RuleFor(x => x.RajalRanap).InclusiveBetween(0, 1);
    }
}