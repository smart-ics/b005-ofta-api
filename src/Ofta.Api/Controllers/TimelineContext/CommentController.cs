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
}