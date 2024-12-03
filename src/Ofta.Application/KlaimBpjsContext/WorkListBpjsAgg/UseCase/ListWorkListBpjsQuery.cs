using Dawn;
using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;

public record ListWorkListBpjsQuery(string? RegId, string? PasienId, string? PasienName,
                                    string? LayananName, string? DokterName, 
                                    string? RajalRanap, string? WorkState, 
                                    int PageNo) : IRequest<IEnumerable<ListWorkListBpjsResponse>>;

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
    private List<WorkListBpjsModel> _resultList = new List<WorkListBpjsModel>();
    private readonly IWorkListBpjsDal _workListBpjsDal;

    public ListWorkListBpjsHandler(IWorkListBpjsDal workListBpjsDal)
    {
        _workListBpjsDal = workListBpjsDal;
    }

    public Task<IEnumerable<ListWorkListBpjsResponse>> Handle(ListWorkListBpjsQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.PageNo, y => y.NotZero());
        
        // QUERY
        if (IsFilterApplied(request))
            _resultList = _workListBpjsDal.ListData()?.ToList()
                ?? new List<WorkListBpjsModel>();
        else
            _resultList = _workListBpjsDal.ListData(request.PageNo)?.ToList()
                ?? new List<WorkListBpjsModel>();

        _resultList = FilterData(request);

        //  RESPONSE
        var response = _resultList.Select(x => new ListWorkListBpjsResponse
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

    private bool IsFilterApplied(ListWorkListBpjsQuery filter)
    {
        return filter.RegId is { Length: >= 3 } ||
               filter.PasienId is { Length: >= 3 } ||
               filter.PasienName is { Length: >= 3 } ||
               filter.LayananName is { Length: >= 3 } ||
               filter.DokterName is { Length: >= 3 } ||
               filter.RajalRanap != null || filter.WorkState != null;
    }

    private List<WorkListBpjsModel> FilterData(ListWorkListBpjsQuery filter)
    {
        if (filter.RegId is { Length: >= 3 })
            _resultList = (from lr in _resultList
                where lr.RegId.ToString().Trim() == filter.RegId.Trim()
                select lr).ToList();

        if (filter.PasienId is { Length: >= 3 })
            _resultList = (from lr in _resultList
                where lr.PasienId.ToString().Trim() == filter.PasienId.Trim()
                select lr).ToList();

        if (filter.PasienName is { Length: >= 3 })
            _resultList = (from lr in _resultList
                where lr.PasienName.ToLower().Trim().Contains(filter.PasienName.ToLower().Trim())
                select lr).ToList();

        if (filter.LayananName is { Length: >= 3 })
            _resultList = (from lr in _resultList
                where lr.LayananName.ToLower().Trim().Contains(filter.LayananName.ToLower().Trim())
                select lr).ToList();

        if (filter.DokterName is { Length: >= 3 })
            _resultList = (from lr in _resultList
                where lr.DokterName.ToLower().Trim().Contains(filter.DokterName.ToLower().Trim())
                select lr).ToList();

        if (filter.RajalRanap != null)
            _resultList = (from lr in _resultList
                where lr.RajalRanap.ToString().Trim() == filter.RajalRanap.Trim()
                select lr).ToList();

        if (filter.WorkState != null)
            _resultList = (from lr in _resultList
                where lr.WorkState.ToString().Trim() == filter.WorkState.Trim()
                select lr).ToList();

        return _resultList;
    }
}
