using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.DocContext.BulkSignAgg.UseCases;
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
    
    [HttpPost("RevokeCertificate")]
    public async Task<IActionResult> RevokeCertificate(TilakaRequestRevokeCertificateCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpGet("GetEmailByRegisterId/{registerId}")]
    public async Task<IActionResult> GetEmailByRegisterId(string registerId)
    {
        var query = new TilakaGetEmailByRegistrationIdQuery(registerId);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }
    
    [HttpGet("GetTilakaNameByUserMapping/{userId}/{userType}")]
    public async Task<IActionResult> GetTilakaNameByUserMapping(string userId, string userType)
    {
        var query = new TilakaUserGetByUserMappingQuery(userId, userType);
        var result = await _mediator.Send(query);
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
    
    [HttpPost("RequestBulkSign")]
    public async Task<IActionResult> RequestBulkSign(RequestBulkSignCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk("Done"));
    }

    [HttpGet("GetSignatureInfo/{documentId}")]
    public async Task<IActionResult> GetSignatureInfo(string documentId)
    {
        var query = new SignatureInfoGetQuery(documentId);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }
}