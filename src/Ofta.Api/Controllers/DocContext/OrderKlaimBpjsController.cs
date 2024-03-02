using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext;

[Route("api/[controller]")]
[ApiController]
public class OrderKlaimBpjsController : Controller
{
    private readonly IMediator _mediator;

    public OrderKlaimBpjsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderKlaimBpjsCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpGet("{tglAwal}/{tglAkhir}")]
    public async Task<IActionResult> List(string tglAwal, string tglAkhir)
    {
        var result = await _mediator.Send(new ListOrderKlaimBpjsQuery(tglAwal, tglAkhir));
        return Ok(new JSendOk(result));
    }
}