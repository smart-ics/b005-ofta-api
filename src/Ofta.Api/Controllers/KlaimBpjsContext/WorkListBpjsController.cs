using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.UseCase;

namespace Ofta.Api.Controllers.KlaimBpjsContext;

[Route("api/[controller]")]
[ApiController]
public class WorkListBpjsController : Controller
{
    private readonly IMediator _mediator;

    public WorkListBpjsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{pageNo}/workListBpjs")]
    public async Task<IActionResult> ListProgress(int pageNo,
        [FromQuery] string? regId = null,
        [FromQuery] string? pasienId = null,
        [FromQuery] string? pasienName = null,
        [FromQuery] string? layananName = null,
        [FromQuery] string? dokterName = null,
        [FromQuery] string? rajalRanap = null,
        [FromQuery] string? workState = null,
        [FromQuery] int sortOrder = 0)
    {
        var query = new ListWorkListBpjsQuery(regId, pasienId, pasienName, layananName, dokterName,
            rajalRanap, workState, sortOrder, pageNo);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }

    [HttpGet("workListBpjsRekap")]
    public async Task<IActionResult> RekapProgress([FromQuery] string? regId = null,
        [FromQuery] string? pasienId = null,
        [FromQuery] string? pasienName = null,
        [FromQuery] string? layananName = null,
        [FromQuery] string? dokterName = null,
        [FromQuery] string? rajalRanap = null,
        [FromQuery] string? workState = null)
    {
        var cmd = new ListWorkListBpjsRecapQuery(regId!, pasienId!, pasienName!, layananName!, dokterName!,
            rajalRanap!, workState!);
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
}