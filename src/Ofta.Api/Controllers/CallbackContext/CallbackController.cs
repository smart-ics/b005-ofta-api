using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;

namespace Ofta.Api.Controllers.CallbackContext;

[Route("api/[controller]")]
[ApiController]
public class CallbackController : ControllerBase
{
    private readonly IMediator _mediator;

    public CallbackController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("signstatus")]
    public async Task<IActionResult> ReceiveCallbackSignStatus(ReceiveCallbackSignStatusCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok(new JSendOk("Done"));
    }
}
