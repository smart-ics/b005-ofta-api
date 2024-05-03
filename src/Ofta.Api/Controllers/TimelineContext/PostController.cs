using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.TImelineContext.PostAgg.UseCases;

namespace Ofta.Api.Controllers.TimelineContext;

[Route("api/[controller]")]
[ApiController]
public class PostController : Controller
{
    private readonly IMediator _mediator;

    public PostController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreatePostCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPut("attachDoc")]
    public async Task<IActionResult> AttachDoc(AttachDocPostCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPut("updateMsg")]
    public async Task<IActionResult> UpdateMsg(UpdateMsgPostCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("addVisibility")]
    public async Task<IActionResult> AddVisibility(AddVisibilityPostCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("removeVisibility")]
    public async Task<IActionResult> RemoveVisibility(RemoveVisibilityPostCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("addReact")]
    public async Task<IActionResult> AddReact(AddReactPostCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    [HttpPatch("removeReact")]
    public async Task<IActionResult> RemoveReact(RemoveReactPostCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpGet("{userOftaId}/{pageNo}")]
    public async Task<IActionResult> ListPost(string userOftaId, int pageNo)
    {
        var query = new ListPostQuery(userOftaId, pageNo);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPost(string id)
    {
        var query = new GetPostQuery(id);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }

    [HttpGet("{id}/ListPostReact")]
    public async Task<IActionResult> ListPostReact(string id)
    {
        var query = new ListPostReactQuery(id);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }
}