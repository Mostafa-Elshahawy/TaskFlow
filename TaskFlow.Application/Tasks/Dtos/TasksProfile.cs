using AutoMapper;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Tasks.Dtos;

public class TasksProfile : Profile
{
    public TasksProfile()
    {
        CreateMap<CreateTaskCommand, Task>();

        CreateMap<TaskEntity, TaskDto>();
    }
}
