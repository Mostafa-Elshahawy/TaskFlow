using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Users.Commands.PromoteUser;
using TaskFlow.Application.Users.Queries.GetUsers;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/identity")]
[Authorize]
public class IdentityController(IMediator mediatR) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await mediatR.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpPut("promote")]
    public async Task<IActionResult> PromoteUserToManager([FromBody] PromoteUserToManagerCommand command)
    {
        await mediatR.Send(command);
        return Ok();
    }
}
