using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.UseCases;

[PublicAPI]
public record ListOrderKlaimBpjsQuery(string TglAwal, string TglAkhir) 
    : IRequest<IEnumerable<ListOrderKlaimBpjsResponse>>;

[PublicAPI]
public record ListOrderKlaimBpjsResponse(
    string OrderKlaimBpjsId,
    string OrderKlaimBpjsDate,
    string UserOftaId,
    string RegId,
    string PasienId,
    string PasienName,
    string NoSep,
    string LayananName,
    string DokterName,
    string RajalRanap);

public class ListOrderKlaimBpjsHandler : IRequestHandler<ListOrderKlaimBpjsQuery, IEnumerable<ListOrderKlaimBpjsResponse>>
{
    private readonly IOrderKlaimBpjsDal _orderKlaimBpjsDal;
    private readonly IValidator<ListOrderKlaimBpjsQuery> _guard;

    public ListOrderKlaimBpjsHandler(IOrderKlaimBpjsDal orderKlaimBpjsDal, 
        IValidator<ListOrderKlaimBpjsQuery> guard)
    {
        _orderKlaimBpjsDal = orderKlaimBpjsDal;
        _guard = guard;
    }

    public Task<IEnumerable<ListOrderKlaimBpjsResponse>> Handle(ListOrderKlaimBpjsQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  QUERY
        var periode = new Periode(
            request.TglAwal.ToDate("yyyy-MM-dd"),
            request.TglAkhir.ToDate("yyyy-MM-dd"));
        var result = _orderKlaimBpjsDal.ListData(periode)?.ToList()
            ?? new List<OrderKlaimBpjsModel>();
        
        //  RESPONSE
        var response = result.Select(x => new ListOrderKlaimBpjsResponse
        (
            x.OrderKlaimBpjsId,
            x.OrderKlaimBpjsDate.ToString("yyyy-MM-dd"),
            x.UserOftaId,
            x.RegId,
            x.PasienId,
            x.PasienName,
            x.NoSep,
            x.LayananName,
            x.DokterName,
            x.RajalRanap.ToString()
        ));
        return Task.FromResult(response);
    }
}

public class ListOrderKlaimBpjsGuard : AbstractValidator<ListOrderKlaimBpjsQuery>
{
    public ListOrderKlaimBpjsGuard()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.TglAwal)
            .NotEmpty()
            .Must(x => x.IsValidTgl("yyyy-MM-dd"));
        RuleFor(x => x.TglAkhir)
            .NotEmpty()
            .Must(x => x.IsValidTgl("yyyy-MM-dd"));
    }
}
    