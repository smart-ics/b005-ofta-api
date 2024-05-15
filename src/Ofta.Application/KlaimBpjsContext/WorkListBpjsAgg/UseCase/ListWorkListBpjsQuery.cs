using Dawn;
using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;

public record ListWorkListBpjsQuery(string pasienName,
    string layananName, string dokterName, 
    string rajalRanap, int pageNo) : IRequest<IEnumerable<ListWorkListBpjsResponse>>;

public record ListWorkListBpjsResponse(
    string OrderKlaimBpjsId,
    string OrderKlaimBpjsDate,
    string KlaimBpjsId,
    string WorkState,
    string RegId,
    string PasienId,
    string PasienName,
    string NoSep,
    string LayananName,
    string DokterName,
    string RajalRanap);

public class ListWorkListBpjsHandler : IRequestHandler<ListWorkListBpjsQuery, IEnumerable<ListWorkListBpjsResponse>>
{
    private readonly IWorkListBpjsDal _workListBpjsDal;

    public ListWorkListBpjsHandler(IWorkListBpjsDal workListBpjsDal)
    {
        _workListBpjsDal = workListBpjsDal;
    }

    public Task<IEnumerable<ListWorkListBpjsResponse>> Handle(ListWorkListBpjsQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.pasienName, y => y.NotEmpty())
            .Member(x => x.layananName, y => y.NotEmpty())
            .Member(x => x.dokterName, y => y.NotEmpty());


        //  QUERY
        var result = _workListBpjsDal.ListData(request.pageNo)?.ToList()
        ?? new List<WorkListBpjsModel>();


        if (request.pasienName != null)
            result = (from lr in result
                      where lr.PasienName.ToLower().Contains(request.pasienName.ToLower())
                      select lr).ToList();

        if (request.layananName != null)
            result = (from lr in result
                      where lr.LayananName.ToLower().Contains(request.layananName.ToLower())
                      select lr).ToList();

        if (request.dokterName != null)
            result = (from lr in result
                      where lr.DokterName.ToLower().Contains(request.dokterName.ToLower())
                      select lr).ToList();

        if (request.rajalRanap != null)
            result = (from lr in result
                      where lr.RajalRanap.ToString() == request.rajalRanap
                      select lr).ToList();

        //  RESPONSE
        var response = result.Select(x => new ListWorkListBpjsResponse
        (
            x.OrderKlaimBpjsId,
            x.OrderKlaimBpjsDate.ToString("yyyy-MM-dd"),
            x.KlaimBpjsId,
            x.WorkState.ToString(),
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
