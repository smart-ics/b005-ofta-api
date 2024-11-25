using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record ListMyDocQuery(string UserOftaId, int PageNo) 
    : IRequest<IEnumerable<ListMyDocResponse>>, IUserOftaKey;

public record ListMyDocResponse(
    string DocId,
    string DocDate,
    string DocTypeId,
    string DocTypeName,
    string DocName,
    DocStateEnum DocState,
    string RequestedDocUrl,
    IEnumerable<DocSigneeResponse> Signees
);

public record DocSigneeResponse(
    string UserOftaId,
    string Email,
    string SignPosition,
    bool IsHidden
);

public class ListMyDocHandler : IRequestHandler<ListMyDocQuery, IEnumerable<ListMyDocResponse>>
{
    private readonly IDocDal _docDal;
    private readonly ITeamUserOftaDal _teamUserOftaDal;
    private readonly IValidator<ListMyDocQuery> _guard;
    private readonly IDocBuilder _docBuilder;

    public ListMyDocHandler(IDocDal docDal, IValidator<ListMyDocQuery> guard, ITeamUserOftaDal teamUserOftaDal, IDocBuilder docBuilder)
    {
        _docDal = docDal;
        _guard = guard;
        _teamUserOftaDal = teamUserOftaDal;
        _docBuilder = docBuilder;
    }

    public Task<IEnumerable<ListMyDocResponse>> Handle(ListMyDocQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        // QUERY
        // combine TEAM and USER => scope
        // 1. Scan Team
        var allTeam = _teamUserOftaDal.ListData(request.UserOftaId)?.ToList()
            ?? new List<TeamUserOftaModel>();
        var scopeReffs = allTeam.Select(x => x.TeamId)?.ToList()
            ?? new List<string>();
        
        // 2. Add Current User
        scopeReffs.Add(request.UserOftaId);
        
        // 3. Add "Public" keyword
        scopeReffs.Add("PUBLIC");
        
        // retrieve data
        var listDoc = _docDal.ListData(scopeReffs, request.PageNo)?.ToList()
            ?? new List<DocModel>();
        
        //  RETURN
        var response = listDoc.Select(BuildResponse);
        return Task.FromResult(response);
    }

    private ListMyDocResponse BuildResponse(DocModel doc)
    {
        var docDetail = _docBuilder
            .Load(doc)
            .Build();

        var signees =
            docDetail.ListSignees.Select(x => new DocSigneeResponse(x.UserOftaId, x.Email, x.SignPosition.ToString(), x.IsHidden));
        
        var newListMyDocResponse = new ListMyDocResponse(
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

public class ListMyDocGuard : AbstractValidator<ListMyDocQuery>
{
    public ListMyDocGuard()
    {
        RuleFor(x => x.PageNo).GreaterThan(0);
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}