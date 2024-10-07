using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.DocContext.DocAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext.TilakaIntegration;

[Route("api/[controller]")]
[ApiController]
public class TilakaController : Controller
{
    private readonly IMediator _mediator;

    public TilakaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("ExecuteSign")]
    public async Task<IActionResult> ExecuteSign(SignDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
   
    [HttpPost("DownloadDoc")]
    public async Task<IActionResult> DownloadFile(PublishDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
}