using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Tasks.Commands.CreateTask;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/tasks/{userId}/tasks")]
[Authorize]
public class TasksController(IMediator _mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskCommand command)
    {
        int id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTaskById), new { id }, null);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(id));
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateTaskStatus(int id, UpdateTaskStatusCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id}/assignee")]
    public async Task<ActionResult> AssignTask(int id, AssignTaskCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        await _mediator.Send(new DeleteTaskCommand { Id = id });
        return NoContent();
    }
}
