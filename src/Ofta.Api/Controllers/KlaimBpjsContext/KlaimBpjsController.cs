using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.UseCases;

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
    public async Task<IActionResult> CreateDoc(KlaimBpjsCreateCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPost("scanReffId")]
    public async Task<IActionResult> ScanReffId(KlaimBpjsPrintOutScanCommand cmd)
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
    public async Task<IActionResult> AddDocType(KlaimBpjsDocTypeAddCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("removeDocType")]
    public async Task<IActionResult> RemoveDocType(KlaimBpjsDocTypeRemoveCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("reOrderDoc")]
    public async Task<IActionResult> ReOrderDoc(KlaimBpjsDocTypeReOrderCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("addPrintOut")]
    public async Task<IActionResult> AddPrintOut(KlaimBpjsPrintOutAddCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("removePrintOut")]
    public async Task<IActionResult> RemovePrintOut(KlaimBpjsPrintOutRemoveCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("printDoc")]
    public async Task<IActionResult> PrintRemoteCetak(KlaimBpjsPrintOutPrintCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("finishPrint")]
    public async Task<IActionResult> FinishPrint(KlaimBpjsPrintOutFinishPrintCallback cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    [HttpPatch("mergerFile")]
    public async Task<IActionResult> MergerFile(KlaimBpjsMergerFileCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpGet("{tglAwal}/{tglAkhir}")]
    public async Task<IActionResult> ListKlaimBpjsMerged(string tglAwal, string tglAkhir,
                                                         [FromQuery] string? rajalRanap = null)
    {
        var result = await _mediator.Send(new ListKlaimBpjsMergedQuery(tglAwal, tglAkhir, rajalRanap!));
        return Ok(new JSendOk(result));
    }
}
