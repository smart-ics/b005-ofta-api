using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration;

[Route("api/[controller]")]
[ApiController]
public class TekenAjaController : Controller
{
    private readonly IMediator _mediator;

    public TekenAjaController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("callback")]
    public async Task<IActionResult> Callback(TekenAjaCallbackRequest callbackRequest)
    {
        var cmd = TekenAjaCallbackFactory
            .Factory(callbackRequest)
            .AdaptCommand(callbackRequest);
        
        await _mediator.Send(cmd);
        return Ok();
    }
}