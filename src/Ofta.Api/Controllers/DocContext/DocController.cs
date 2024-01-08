using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.DocContext.DocAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext;

[Route("api/[controller]")]
[ApiController]
public class DocController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<DocController> _logger;

    public DocController(IMediator mediator, 
        ILogger<DocController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDoc(CreateDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("addSignee")]
    public async Task<IActionResult> AddSigneeDoc(AddSigneeDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("removeSignee")]
    public async Task<IActionResult> RemoveSigneeDoc(RemoveSigneeDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("submit")]
    public async Task<IActionResult> GenDoc(SubmitDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    
}