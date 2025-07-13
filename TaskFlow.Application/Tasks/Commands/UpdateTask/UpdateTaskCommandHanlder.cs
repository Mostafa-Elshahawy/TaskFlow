using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHanlder(ILogger<UpdateTaskCommandHanlder> logger, ITaskRepository taskRepository, IMapper mapper)
                : IRequestHandler<UpdateTaskCommand>
{
    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating Task ID: {Id}", request.Id);
        var task = await taskRepository.GetById(request.Id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID: {request.Id} not found");
        }

        mapper.Map(request, task);

        await taskRepository.SaveChanges();
    }
}
