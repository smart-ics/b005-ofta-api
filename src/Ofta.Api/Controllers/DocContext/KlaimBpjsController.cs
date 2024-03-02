using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext;

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
}
