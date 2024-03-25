using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.UserContext.TeamAgg.UseCases;

namespace Ofta.Api.Controllers.UserContext;

[Route("api/[controller]")]
[ApiController]
public class TeamController : Controller
{
    private readonly IMediator _mediator;

    public TeamController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam(CreateTeamCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }

    [HttpPatch("addMember")]
    public async Task<IActionResult> AddMember(AddMemberTeamCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    [HttpPatch("removeMember")]
    public async Task<IActionResult> RemoveMember(RemoveMemberTeamCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
}