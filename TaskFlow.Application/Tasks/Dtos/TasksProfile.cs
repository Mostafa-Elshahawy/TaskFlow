using AutoMapper;
using TaskFlow.Application.Tasks.Commands.CreateTask;

namespace TaskFlow.Application.Tasks.Dtos;

public class TasksProfile : Profile
{
    public TasksProfile()
    {
        CreateMap<CreateTaskCommand, Task>();

        CreateMap<Task, TaskDto>();
    }
}
