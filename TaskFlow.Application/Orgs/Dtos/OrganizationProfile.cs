using AutoMapper;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Orgs.Dtos;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<Organization, OrganizationDto>();
    }
}
