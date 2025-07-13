using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler(ILogger<DeleteTaskCommandHandler> logger, ITaskRepository taskRepository)
                : IRequestHandler<DeleteTaskCommand>
{
    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting Task ID {TaskId}", request.Id);
        var task = await taskRepository.GetById(request.Id);
        if (task is null)
        {
            throw new NotFoundException(nameof(task), request.Id.ToString());
        }

        await taskRepository.Delete(task);
    }
}
