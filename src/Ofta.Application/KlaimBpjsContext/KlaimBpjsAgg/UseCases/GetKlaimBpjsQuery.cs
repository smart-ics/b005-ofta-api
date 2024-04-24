using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record GetKlaimBpjsQuery(string KlaimBpjsId) : 
    IRequest<GetKlaimBpjsResponse>, IKlaimBpjsKey;
# region RESPONSE

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
    string RajalRanap,
    List<GetKlaimBpjsDocResponse> ListDoc,
    List<GetKlaimBpjsEventResponse> ListEvent
);

public class GetKlaimBpjsDocResponse
{
    public string KlaimBpjsDocId { get; set; }
    public int NoUrut { get; set; }
    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    public string DocId { get; set; }
    public string DocUrl { get; set; }
    public string PrintOutReffId { get; set; }
}

public class GetKlaimBpjsEventResponse
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsJurnalId { get; set; }
    public int NoUrut { get; set; }
    public string EventDate { get; set; }
    public string Description { get; set; }
}

#endregion
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
            klaimBpjs.RajalRanap.ToString().ToUpper(),
            klaimBpjs.ListDocType
                     .Select(x => new GetKlaimBpjsDocResponse
                     {
                         KlaimBpjsDocId = x.KlaimBpjsDocTypeId,
                         NoUrut = x.NoUrut,
                         DocTypeId = x.DocTypeId,
                         DocTypeName = x.DocTypeName,
                         DocId = string.Empty,//x.DocId,
                         DocUrl = string.Empty,//x.DocUrl,
                         PrintOutReffId = string.Empty //x.PrintOutReffId
                     }).ToList(),
            klaimBpjs.ListEvent
                        .Select(x => new GetKlaimBpjsEventResponse
                        {
                            KlaimBpjsId = x.KlaimBpjsId,
                            KlaimBpjsJurnalId = x.KlaimBpjsJurnalId,
                            NoUrut = x.NoUrut,
                            EventDate = x.EventDate.ToString("yyyy-MM-dd hh:mm:ss"),
                            Description = x.Description
                        }
                        ).ToList()
        );

      
        return Task.FromResult(response);
    }
}
