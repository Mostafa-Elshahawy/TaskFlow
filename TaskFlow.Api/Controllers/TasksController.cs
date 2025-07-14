using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Application.Tasks.Commands.DeleteTask;
using TaskFlow.Application.Tasks.Commands.UpdateTask;
using TaskFlow.Application.Tasks.Queries.GetTaskById;
using TaskFlow.Application.Tasks.Queries.GetTasks;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/tasks")]
//[Authorize]
public class TasksController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskCommand command)
    {
        int id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetTaskById), new { id }, null);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var result = await mediator.Send(new GetTaskByIdQuery(id));
        return Ok(result);
    }


    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateTask(int id, UpdateTaskCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        await mediator.Send(new DeleteTaskCommand(id));
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredTasks([FromQuery] GetTasksQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}