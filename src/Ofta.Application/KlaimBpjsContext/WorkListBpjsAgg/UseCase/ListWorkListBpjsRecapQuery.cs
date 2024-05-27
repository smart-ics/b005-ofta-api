using Dawn;
using Mapster;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;


public record ListWorkListBpjsRecapQuery(string regId, string pasienId, string pasienName,
                                         string layananName, string dokterName,
                                         string rajalRanap, string workState) 
                                        : IRequest<IEnumerable<ListWorkListBpjsRecapResponse>>;


public record ListWorkListBpjsRecapResponse(
    string Keterangan,
    int Count);


public class ListWorkListBpjsRecapHandler : IRequestHandler<ListWorkListBpjsRecapQuery, IEnumerable<ListWorkListBpjsRecapResponse>>
{
    private readonly IWorkListBpjsDal _workListBpjsDal;

    public ListWorkListBpjsRecapHandler(IWorkListBpjsDal workListBpjsDal)
    {
        _workListBpjsDal = workListBpjsDal;
    }

    public Task<IEnumerable<ListWorkListBpjsRecapResponse>> Handle(ListWorkListBpjsRecapQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.pasienName, y => y.NotEmpty())
            .Member(x => x.layananName, y => y.NotEmpty())
            .Member(x => x.dokterName, y => y.NotEmpty());


        //  QUERY
        var result = _workListBpjsDal.ListData()?.ToList() 
                  ?? new List<WorkListBpjsModel>();

        if (request.regId != null)
            result = result.Where(lr => lr.RegId.ToString().Trim() == request.regId.Trim()).ToList();

        if (request.pasienId != null)
            result = result.Where(lr => lr.PasienId.ToString().Trim() == request.pasienId.Trim()).ToList();

        if (request.pasienName != null)
            result = result.Where(lr => lr.PasienName.ToLower().Trim().Contains(request.pasienName.ToLower().Trim())).ToList();

        if (request.layananName != null)
            result = result.Where(lr => lr.LayananName.ToLower().Trim().Contains(request.layananName.ToLower().Trim())).ToList();

        if (request.dokterName != null)
            result = result.Where(lr => lr.DokterName.ToLower().Trim().Contains(request.dokterName.ToLower().Trim())).ToList();

        if (request.rajalRanap != null)
            result = result.Where(lr => lr.RajalRanap.ToString().Trim() == request.rajalRanap.Trim()).ToList();

        if (request.workState != null)
            result = result.Where(lr => lr.WorkState.ToString().Trim() == request.workState.Trim()).ToList();


        int totalCount = result.Count;              // total

        var resultStateRecap = result               // total per state
            .GroupBy(d => d.WorkState.ToString())
            .Select(g => new
            {
                WorkState = g.Key,
                RecapWorkState = g.Count()
            }).ToList();

        //  RESPONSE
        var dataList = resultStateRecap.Select(x => new ListWorkListBpjsRecapResponse
        (
            x.WorkState,
            x.RecapWorkState
        )).ToList();

        var total = new ListWorkListBpjsRecapResponse("Total", totalCount);
        
        dataList.Add(total);

        var response = dataList.Adapt<IEnumerable<ListWorkListBpjsRecapResponse>>();
        return Task.FromResult(response);
    }
}
