using Dawn;
using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;

public record ListWorkListBpjsQuery(string regId, string pasienId, string pasienName,
                                    string layananName, string dokterName, 
                                    string rajalRanap, string workState, 
                                    int pageNo) : IRequest<IEnumerable<ListWorkListBpjsResponse>>;

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


        if (request.regId != null)
            result = (from lr in result
                      where lr.RegId.ToString().Trim() == request.regId.Trim()
                      select lr).ToList();

        if (request.pasienId != null)
            result = (from lr in result
                      where lr.PasienId.ToString().Trim() == request.pasienId.Trim()
                      select lr).ToList();

        if (request.pasienName != null)
            result = (from lr in result
                      where lr.PasienName.ToLower().Trim().Contains(request.pasienName.ToLower().Trim())
                      select lr).ToList();

        if (request.layananName != null)
            result = (from lr in result
                      where lr.LayananName.ToLower().Trim().Contains(request.layananName.ToLower().Trim())
                      select lr).ToList();

        if (request.dokterName != null)
            result = (from lr in result
                      where lr.DokterName.ToLower().Trim().Contains(request.dokterName.ToLower().Trim())
                      select lr).ToList();

        if (request.rajalRanap != null)
            result = (from lr in result
                      where lr.RajalRanap.ToString().Trim() == request.rajalRanap.Trim()
                      select lr).ToList();

        if (request.workState != null)
            result = (from lr in result
                      where lr.WorkState.ToString().Trim() == request.workState.Trim()
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
