using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.TImelineContext.CommentAgg.UseCases;

namespace Ofta.Api.Controllers.TimelineContext;

[Route("api/[controller]")]
[ApiController]
public class CommentController : Controller
{
    private readonly IMediator _mediator;

    public CommentController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }    

    [HttpPut("updateMsg")]
    public async Task<IActionResult> UpdateMsg(UpdateMsgCommentCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }    

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCommentCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpPatch("addReact")]
    public async Task<IActionResult> AddReact(AddReactCommentCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }    

    [HttpPatch("removeReact")]
    public async Task<IActionResult> RemoveReact(RemoveReactCommentCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpGet("{PostId}/ListComment")]
    public async Task<IActionResult> ListComment(string PostId)
    {
        var query = new ListCommentQuery(PostId);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComment(string id)
    {
        var query = new GetCommentQuery(id);
        var result = await _mediator.Send(query);
        return Ok(new JSendOk(result));
    }
}