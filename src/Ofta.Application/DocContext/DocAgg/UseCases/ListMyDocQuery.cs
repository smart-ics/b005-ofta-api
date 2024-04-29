using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
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
    string RequestedDocUrl);

public class ListMyDocHandler : IRequestHandler<ListMyDocQuery, IEnumerable<ListMyDocResponse>>
{
    private readonly IDocDal _docDal;
    private readonly ITeamUserOftaDal _teamUserOftaDal;
    
    private readonly IValidator<ListMyDocQuery> _guard;

    public ListMyDocHandler(IDocDal docDal, IValidator<ListMyDocQuery> guard, ITeamUserOftaDal teamUserOftaDal)
    {
        _docDal = docDal;
        _guard = guard;
        _teamUserOftaDal = teamUserOftaDal;
    }

    public Task<IEnumerable<ListMyDocResponse>> Handle(ListMyDocQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        //  QUERY
        //      combine TEAM and USER => scope
        //      1. Scan Team
        var allTeam = _teamUserOftaDal.ListData(request.UserOftaId)?.ToList()
            ?? new List<TeamUserOftaModel>();
        var scopeReffs = allTeam.Select(x => x.TeamId)?.ToList()
            ?? new List<string>();
        //      2. Add Current User
        scopeReffs.Add(request.UserOftaId);
        //      3. Add "Public" keyword
        scopeReffs.Add("PUBLIC");
        //      retrieve data
        var listDoc = _docDal.ListData(scopeReffs, request.PageNo)?.ToList()
            ?? new List<DocModel>();
        
        //  RETURN
        var response = listDoc.Select(x => new ListMyDocResponse
            (
                x.DocId, 
                $"{x.DocDate:yyyy-MM-dd HH:mm:ss}",
                x.DocTypeId,
                x.DocTypeName,
                x.DocName,
                x.RequestedDocUrl
            ));
        return Task.FromResult(response);
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