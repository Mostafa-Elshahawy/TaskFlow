using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Orgs.Commnads.AcceptOrganizationInvitation;
using TaskFlow.Application.Orgs.Commnads.AddOrganizationMembers;
using TaskFlow.Application.Orgs.Commnads.CreateOrganization;
using TaskFlow.Application.Orgs.Queries.GetOrganizations;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetOrganizationsQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(CreateOrgCommand command)
    {
        var orgId = await mediator.Send(command);
        return Ok(orgId);
    }

    [HttpPost("send-invite")]
    public async Task<IActionResult> InviteMember(InviteOrganizationMembersCommand command)
    {
        var inviteId = await mediator.Send(command);
        return Ok(inviteId);
    }

    [HttpPost("accept-invite")]
    public async Task<IActionResult> AcceptInvite(
        [FromBody] AcceptOrganizationInvitationCommand command,
        CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
