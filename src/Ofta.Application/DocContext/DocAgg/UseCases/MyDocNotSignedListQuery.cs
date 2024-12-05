using Dawn;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record MyDocNotSignedListQuery(string UserOftaId)
    : IRequest<IEnumerable<MyDocNotSignedListResponse>>, IUserOftaKey;

public record MyDocNotSignedListResponse(
    string DocId,
    string DocDate,
    string DocTypeId,
    string DocTypeName,
    string DocName,
    DocStateEnum DocState,
    string RequestedDocUrl,
    IEnumerable<MyDocNotSignedSigneeResponse> Signees
);

public record MyDocNotSignedSigneeResponse(
    string UserOftaId,
    string Email,
    string SignPosition,
    SignStateEnum SignState,
    bool IsHidden
);

public class MyDocNotSignedListHandler: IRequestHandler<MyDocNotSignedListQuery, IEnumerable<MyDocNotSignedListResponse>>
{
    private readonly IDocDal _docDal;
    private readonly ITeamUserOftaDal _teamUserOftaDal;
    private readonly IDocBuilder _docBuilder;

    public MyDocNotSignedListHandler(IDocDal docDal, ITeamUserOftaDal teamUserOftaDal, IDocBuilder docBuilder)
    {
        _docDal = docDal;
        _teamUserOftaDal = teamUserOftaDal;
        _docBuilder = docBuilder;
    }

    public Task<IEnumerable<MyDocNotSignedListResponse>> Handle(MyDocNotSignedListQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.UserOftaId, y => y.NotEmpty());
        
        // QUERY
        var allTeam = _teamUserOftaDal.ListData(request.UserOftaId)?.ToList()
            ?? new List<TeamUserOftaModel>();
        var scopeReffs = allTeam.Select(x => x.TeamId).ToList();
        scopeReffs.Add(request.UserOftaId);
        scopeReffs.Add("PUBLIC");
        
        var listDoc = _docDal.ListData(scopeReffs)
            ?? new List<DocModel>();
        
        // RESPONSE
        var response = listDoc
            .Where(x => x.ListSignees.IsNotEmpty() && x.ListSignees.Any(
                y => y.UserOftaId == request.UserOftaId && y.SignState == SignStateEnum.NotSigned))
            .Select(BuildResponse);
        
        return Task.FromResult(response);
    }
    
    private MyDocNotSignedListResponse BuildResponse(DocModel doc)
    {
        var docDetail = _docBuilder
            .Load(doc)
            .Build();

        var signees =
            docDetail.ListSignees.Select(x => new MyDocNotSignedSigneeResponse(x.UserOftaId, x.Email, x.SignPosition.ToString(), x.SignState, x.IsHidden));
        
        var newListMyDocResponse = new MyDocNotSignedListResponse(
            doc.DocId,
            $"{doc.DocDate:yyyy-MM-dd HH:mm:ss}",
            doc.DocTypeId,
            doc.DocTypeName,
            doc.DocName,
            doc.DocState,
            doc.RequestedDocUrl,
            signees
        );

        return newListMyDocResponse;
    }
}