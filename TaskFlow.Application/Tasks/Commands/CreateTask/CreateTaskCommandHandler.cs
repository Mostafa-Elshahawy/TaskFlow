using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Users;
using TaskFlow.Domain.Repositories;
using Task = TaskFlow.Domain.Entites.Task;

namespace TaskFlow.Application.Tasks.Commands.CreateTask;

internal class CreateTaskCommandHandler(ILogger logger,
                                        ITaskRepository taskRepository,
                                        IMapper mapper,
                                        IUserContext userContext)
                                       :IRequestHandler<CreateTaskCommand, int>
{
    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("{UserEmail} [{UserId}] is creating a new task {@Task}", 
            currentUser.Email,
            currentUser.Id,
            request);

        var task = mapper.Map<Task>(request);

        int id = await taskRepository.Create(task);
        return id;
    }
}
