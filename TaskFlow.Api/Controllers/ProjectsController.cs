using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Application.Projects.Commands.DeleteProject;
using TaskFlow.Application.Projects.Commands.UpdateProject;
using TaskFlow.Application.Projects.Queries.GetProjectById;
using TaskFlow.Application.Projects.Queries.GetProjects;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProjects(GetProjectsQuery query)
    {
        var result = await mediator.Send(new GetProjectsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var result = await mediator.Send(new GetProjectByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectCommand command)
    {
        int id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetProjectById), new { id }, null);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateProject(int id, UpdateProjectCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await mediator.Send(new DeleteProjectCommand(id));
        return NoContent();
    }
}