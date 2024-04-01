using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

namespace Ofta.Api.Controllers.KlaimBpjsContext;

[Route("api/[controller]")]
[ApiController]
public class KlaimBpjsController : Controller
{
    private readonly IMediator _mediator;

    public KlaimBpjsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDoc(CreateKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetKlaimBpjs(string id)
    {
        var query = new GetKlaimBpjsQuery(id);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("addDocType")]
    public async Task<IActionResult> AddDocType(AddDocTypeKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("removeDocType")]
    public async Task<IActionResult> RemoveDocType(RemoveDocTypeKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("reOrderDoc")]
    public async Task<IActionResult> ReOrderDoc(ReOrderDocKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("printOutReffId")]
    public async Task<IActionResult> SetPrintReffId(SetPrintReffIdKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("printDoc")]
    public async Task<IActionResult> PrintRemoteCetak(PrintDocKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("finishPrint")]
    public async Task<IActionResult> FinishPrint(FinishPrintDocKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

}
