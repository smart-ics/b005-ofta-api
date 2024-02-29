﻿using MediatR;
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
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPut("fileUrl")]
    public async Task<IActionResult> FileUrlDocType(FileUrlDocTypeCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }

    [HttpPut("activate")]
    public async Task<IActionResult> ActivateDocType(ActivateDocTypeCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }

    [HttpPut("deactivate")]
    public async Task<IActionResult> DeactivateDocType(DeactivateDocTypeCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }

    [HttpPatch("addTag")]
    public async Task<IActionResult> AddTagDocType(AddTagDocTypeCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }
    [HttpPatch("removeTag")]
    public async Task<IActionResult> RemoveTagDocType(RemoveTagDocTypeCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }
    [HttpGet("{tag}/list")]
    public async Task<IActionResult> ListDocType(string tag)
    {
        var result = await _mediator.Send(new ListDocTypeQuery(tag));
        return Ok(new JSendOk(result));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDocType(string id)
    {
        var result = await _mediator.Send(new GetDocTypeQuery(id));
        return Ok(new JSendOk(result));
    }
}