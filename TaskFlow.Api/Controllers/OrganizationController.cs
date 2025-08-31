using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Orgs.Commnads;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationController(IMediator mediator) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(CreateOrgCommand command)
    {
        var orgId = await mediator.Send(command);
        return Ok(orgId);
    }
}
