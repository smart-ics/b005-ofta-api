using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nuna.Lib.ActionResultHelper;
using Ofta.Application.UserContext.UserOftaAgg.UseCases;

namespace Ofta.Api.Controllers.UserContext;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(new JSendOk(result));
    }
    
    [HttpGet("{email}")]
    public async Task<IActionResult> GetUser(string email)
    {
        var result = await _mediator.Send(new GetUserQuery(email));
        return Ok(new JSendOk(result));
    }
    
    [HttpGet]
    public async Task<IActionResult> ListUser()
    {
        var result = await _mediator.Send(new ListUserQuery());
        return Ok(new JSendOk(result));
    }
}