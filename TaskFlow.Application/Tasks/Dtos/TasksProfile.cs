using AutoMapper;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Tasks.Dtos;

public class TasksProfile : Profile
{
    public TasksProfile()
    {
        CreateMap<CreateTaskCommand, TaskEntity>()
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.Assignee, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.isDeleted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<TaskEntity, TaskDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.UserName))
            .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee != null ? src.Assignee.UserName : null));
    }
}
