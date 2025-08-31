using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Domain.Entites;

public class ApplicationUser : IdentityUser
{
    public ICollection<Organization> OwnedOrganizations { get; set; } = default!;
    public ICollection<OrganizationMember> Organizations { get; set; } = new List<OrganizationMember>();
    public ICollection<Project> AssignedProjects { get; set; } = default!;
    public ICollection<Project> ManagedProjects { get; set; } = default!;
    public ICollection<TaskEntity> AssignedTasks { get; set; } = default!;
    public ICollection<TaskEntity> CreatedTasks { get; set; } = default!;
}
