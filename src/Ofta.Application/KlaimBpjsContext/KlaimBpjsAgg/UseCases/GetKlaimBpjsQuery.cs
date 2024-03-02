using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record GetKlaimBpjsQuery(string KlaimBpjsId) : 
    IRequest<GetKlaimBpjsResponse>, IKlaimBpjsKey;

public record GetKlaimBpjsResponse(
    string KlaimBpjsId,
    string KlaimBpjsDate,
    string KlaimBpjsState,
    string RegId,
    string PasienId,
    string PasienName,
    string NoSep,
    string LayananName,
    string DokterName,
    string RajalRanap
);

public class GetKlaimBpjsQueryHandler : IRequestHandler<GetKlaimBpjsQuery, GetKlaimBpjsResponse>
{
    private readonly IKlaimBpjsBuilder _builder;

    public GetKlaimBpjsQueryHandler(IKlaimBpjsBuilder builder)
    {
        _builder = builder;
    }

    public Task<GetKlaimBpjsResponse> Handle(GetKlaimBpjsQuery request, CancellationToken cancellationToken)
    {
        var klaimBpjs = _builder.Load(request).Build();
        var response = new GetKlaimBpjsResponse(
            klaimBpjs.KlaimBpjsId,
            klaimBpjs.KlaimBpjsDate.ToString("yyyy-MM-dd"),
            klaimBpjs.KlaimBpjsState.ToString().ToUpper(),
            klaimBpjs.RegId,
            klaimBpjs.PasienId,
            klaimBpjs.PasienName,
            klaimBpjs.NoSep,
            klaimBpjs.LayananName,
            klaimBpjs.DokterName,
            klaimBpjs.RajalRanap.ToString().ToUpper()
        );
        return Task.FromResult(response);
    }
}
