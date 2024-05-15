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
                                                  [FromQuery] string? pasienName = null,
                                                  [FromQuery] string? layananName = null,
                                                  [FromQuery] string? dokterName = null,
                                                  [FromQuery] string? rajalRanap = null)
    {
        var cmd = new ListWorkListBpjsQuery(pasienName!, layananName!, dokterName!, rajalRanap!, pageNo);
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

}