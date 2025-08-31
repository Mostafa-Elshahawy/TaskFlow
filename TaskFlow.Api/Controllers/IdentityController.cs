using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Users.Commands.PromoteUser;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/identity")]
[Authorize]
public class IdentityController(IMediator mediatR) : ControllerBase
{
    [HttpPut]
    [Route("promote")]
    public async Task<IActionResult> PromoteUserToManager([FromBody] PromoteUserToManagerCommand command)
    {
        await mediatR.Send(command);
        return Ok();
    }
}
