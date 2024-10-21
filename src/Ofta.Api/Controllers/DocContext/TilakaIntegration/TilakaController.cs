using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.DocContext.DocAgg.UseCases;
using Ofta.Application.UserContext.TilakaAgg.UseCases;

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

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser(TilakaRegistrationCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPost("CheckExistingAccount")]
    public async Task<IActionResult> CheckExistingAccount(TilakaCheckExistingAccountCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPost("CheckUserRegistration")]
    public async Task<IActionResult> CheckUserRegistration(TilakaCheckUserRegistrationCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPost("CheckUserCertificate")]
    public async Task<IActionResult> CheckUserCertificate(TilakaCheckUserCertificateCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPost("ExecuteSign")]
    public async Task<IActionResult> ExecuteSign(SignDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPost("RejectSign")]
    public async Task<IActionResult> RejectSign(RejectSignDocCommand cmd)
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