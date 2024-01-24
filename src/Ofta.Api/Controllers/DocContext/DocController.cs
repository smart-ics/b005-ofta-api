﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.DocContext.DocAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext;

[Route("api/[controller]")]
[ApiController]
public class DocController : Controller
{
    private readonly IMediator _mediator;

    public DocController(IMediator mediator)
    {
        _mediator = mediator;
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

    [HttpPut("submit")]
    public async Task<IActionResult> GenDoc(SubmitDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPut("sign")]
    public async Task<IActionResult> SignDoc(SignDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPut("publish")]
    public async Task<IActionResult> PublishDoc(PublishDocCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDataDoc(string id)
    {
        var query = new GetDocQuery(id);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }

    [HttpGet("{email}/{tglYmd1}/{tglYmd2}")]
    public async Task<IActionResult> GetDataDoc(string email, string tglYmd1, string tglYmd2)
    {
        var query = new ListDocQuery(email, tglYmd1, tglYmd2);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }
}