using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;


namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

[PublicAPI]
public record ListKlaimBpjsMergedQuery(string TglAwal, string TglAkhir, string? RajalRanap, string? DokterName)
    : IRequest<IEnumerable<ListKlaimBpjsMergedResponse>>;

[PublicAPI]
public record ListKlaimBpjsMergedResponse(
    string KlaimBpjsId,
    string KlaimBpjsDate,
    string KlaimBpjsState,
    string RegId,
    string PasienId,
    string PasienName,
    string NoSep,
    string LayananName,
    string DokterName,
    string RajalRanap,
    string MergerDocUrl);

public class ListKlaimBpjsHandler : IRequestHandler<ListKlaimBpjsMergedQuery, IEnumerable<ListKlaimBpjsMergedResponse>>
{
    private readonly IKlaimBpjsDal _klaimBpjsDal;
    private readonly IValidator<ListKlaimBpjsMergedQuery> _guard;

    public ListKlaimBpjsHandler(IKlaimBpjsDal klaimBpjsDal,
        IValidator<ListKlaimBpjsMergedQuery> guard)
    {
        _klaimBpjsDal = klaimBpjsDal;
        _guard = guard;
    }

    public Task<IEnumerable<ListKlaimBpjsMergedResponse>> Handle(ListKlaimBpjsMergedQuery request,
        CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);

        //  QUERY
        var periode = new Periode(
            request.TglAwal.ToDate("yyyy-MM-dd"),
            request.TglAkhir.ToDate("yyyy-MM-dd"));

        var listKlaimBpjs = _klaimBpjsDal.ListData(periode)?.ToList() ?? [];
        var result = listKlaimBpjs
            .Where(x => x.KlaimBpjsState.ToString() == KlaimBpjsStateEnum.Merged.ToString()).ToList();

        if (request.DokterName is { Length: >= 3 })
            result = (from lr in result
                where lr.DokterName.ToLower().Trim().Contains(request.DokterName.ToLower().Trim())
                select lr).ToList();
        
        if (request.RajalRanap != null)
            result = (from lr in result
                where lr.RajalRanap.ToString().Trim() == request.RajalRanap.Trim()
                select lr).ToList();

        //  RESPONSE
        var response = result.Select(x => new ListKlaimBpjsMergedResponse
        (
            x.KlaimBpjsId,
            x.KlaimBpjsDate.ToString("yyyy-MM-dd"),
            x.KlaimBpjsState.ToString(),
            x.RegId,
            x.PasienId,
            x.PasienName,
            x.NoSep,
            x.LayananName,
            x.DokterName,
            x.RajalRanap.ToString(),
            x.MergerDocUrl
        ));
        return Task.FromResult(response);
    }
}

public class ListKlaimBpjsMergedGuard : AbstractValidator<ListKlaimBpjsMergedQuery>
{
    public ListKlaimBpjsMergedGuard()
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