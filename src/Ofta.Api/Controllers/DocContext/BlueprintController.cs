using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.DocContext.BlueprintAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext;

[Route("api/[controller]")]
[ApiController]
public class BlueprintController : Controller
{
    private readonly IMediator _mediator;

    public BlueprintController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBlueprintCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPut("rename")]
    public async Task<IActionResult> Rename(RenameBlueprintCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }

    [HttpPatch("addDocType")]
    public async Task<IActionResult> AddDocType(AddDocTypeBlueprintCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }
    
    [HttpPatch("removeDocType")]
    public async Task<IActionResult> RemoveDocType(RemoveDocTypeBlueprintCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }
    
    [HttpPatch("addSignee")]
    public async Task<IActionResult> AddSignee(AddSigneeBlueprintCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }
    
    [HttpPatch("removeSignee")]
    public async Task<IActionResult> RemoveSignee(RemoveSigneeBlueprintCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var cmd = new DeleteBlueprintCommand(id);
        await _mediator.Send(cmd);
        return Ok();
    }
}