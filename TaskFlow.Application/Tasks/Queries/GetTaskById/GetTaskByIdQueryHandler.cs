using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Tasks.Dtos;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler(ILogger<GetTaskByIdQueryHandler> logger, ITaskRepository taskRepository, IMapper mapper) : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Task With ID: {TaskId}", request.Id);
        var taskEntity = await taskRepository.GetById(request.Id);
        if (taskEntity == null)
        {
             throw new NotFoundException(nameof(taskEntity), request.Id.ToString());
        }
        var taskDto = mapper.Map<TaskDto>(taskEntity);
        return taskDto;
    }
}
