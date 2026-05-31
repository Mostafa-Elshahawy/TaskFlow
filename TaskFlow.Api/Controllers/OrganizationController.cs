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

    [AllowAnonymous]
    [HttpGet("accept-invite")]
    public async Task<IActionResult> AcceptInvite([FromQuery] string token)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirst("sub")?.Value
            : null;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("You must be logged in to accept the invitation.");

        var command = new AcceptOrganizationInvitationCommand(token, userId);
        await mediator.Send(command);

        return Ok("Invitation accepted successfully.");
    }
}
