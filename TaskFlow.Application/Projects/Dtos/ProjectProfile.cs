using AutoMapper;
using TaskFlow.Application.Projects.Commands.CreateProject;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Projects.Dtos;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<CreateProjectCommand, Project>()
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.isDeleted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.UserName));
    }
}
