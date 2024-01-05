using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.DocContext.DocTypeAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext;

[Route("api/[controller]")]
[ApiController]
public class DocTypeController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<DocTypeController> _logger;
    public DocTypeController(IMediator mediator, ILogger<DocTypeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDocType(CreateDocTypeCommand cmd)
    {
        _logger.LogInformation("CreateDocType executed with {@cmd}", cmd);
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
}