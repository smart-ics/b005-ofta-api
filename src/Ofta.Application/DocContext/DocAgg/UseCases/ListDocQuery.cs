using Dawn;
using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record ListDocQuery(string Email, string TglYmd1, string TglYmd2) : IRequest<IEnumerable<ListDocResponse>>;

public record ListDocResponse(
    string DocId,
    string DocDate,
    string DocTypeId,
    string DocTypeName,
    string DocName,
    string UserOftaId,
    string Email,
    string DocState);

public class ListDocQueryHandler : IRequestHandler<ListDocQuery, IEnumerable<ListDocResponse>>
{
    private readonly IDocDal _docDal;
    private readonly IUserOftaDal _userOftaDal;

    public ListDocQueryHandler(IDocDal docDal, IUserOftaDal userOftaDal)
    {
        _docDal = docDal;
        _userOftaDal = userOftaDal;
    }

    public Task<IEnumerable<ListDocResponse>> Handle(ListDocQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.Email, x => x.NotEmpty());
        
        //  QUERY
        var userOfta = _userOftaDal.GetData(request.Email)
                       ?? throw new KeyNotFoundException("Email not found");
        var periode = new Periode(request.TglYmd1.ToDate(),
            request.TglYmd2.ToDate());
        var list = _docDal.ListData(periode, userOfta)?.ToList()
            ?? new List<DocModel>();
        
        //  PROJECTION
        var response = list.Select(x => new ListDocResponse(
            x.DocId,
            x.DocDate.ToString("yyyy-MM-dd"),
            x.DocTypeId,
            x.DocTypeName,
            x.DocName,
            x.UserOftaId,
            x.Email,
            x.DocState.ToString()));
        return Task.FromResult(response);
    }
}
