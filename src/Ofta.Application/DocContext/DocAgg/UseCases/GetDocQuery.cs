using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record GetDocQuery(string DocId) : IRequest<GetDocResponse>,IDocKey;

public record GetDocResponse(
    string DocId,
    string DocDate,
    string DocTypeId,
    string DocTypeName,
    
    string DocOwnerUserOftaId,
    string DocOwnerEmail,
    
    string DocState,
    string DocName,
    string RequestedDocUrl,
    string UploadedDocId,
    string UploadedDocUrl,
    string PublishedDocUrl,
    List<GetDocResponseSignee> ListSignees,
    List<GetDocResponseJurnal> ListJurnal
);

public record GetDocResponseJurnal(
    int NoUrut,
    string JurnalDate,
    string JurnalDesc);

public record GetDocResponseSignee(

    string SigneeUserOftaId,
    string SigneeName,
    string SigneeEmail,
    string SignTag,
    string SignPosition,
    int Level,
    string SignState,
    string SignedDate,
    string SignPositionDesc,
    string SignUrl);

public class GetDocQueryHandler : IRequestHandler<GetDocQuery, GetDocResponse>
{
    private readonly IDocBuilder _builder;

    public GetDocQueryHandler(IDocBuilder builder)
    {
        _builder = builder;
    }

    public Task<GetDocResponse> Handle(GetDocQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, x => x.NotEmpty());
        
        //  BUILD
        var doc = _builder
            .Load(request)
            .Build();
        
        //  PROJECTION
        var response = new GetDocResponse(
            doc.DocId,
            doc.DocDate.ToString("yyyy-MM-dd"),
            doc.DocTypeId,
            doc.DocTypeName,
            doc.UserOftaId,
            doc.Email,
            doc.DocState.ToString(),
            doc.DocName,
            doc.RequestedDocUrl,
            doc.UploadedDocId,
            doc.UploadedDocUrl,
            doc.PublishedDocUrl,
            doc.ListSignees.Select(x => new GetDocResponseSignee(
                x.UserOftaId,
                x.Email,
                x.Email,
                x.SignTag,
                x.SignPosition.ToString(),
                x.Level,
                x.SignState.ToString(),
                x.SignedDate.ToString("yyyy-MM-dd"),
                x.SignPositionDesc,
                x.SignUrl
            )).ToList(),
            doc.ListJurnal.Select(x => new GetDocResponseJurnal(
                x.NoUrut,
                x.JurnalDate.ToString("yyyy-MM-dd"),
                x.JurnalDesc
            )).ToList()
        );
        return Task.FromResult(response);
    }
}
