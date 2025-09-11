using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Domain.Entites;

public class ApplicationUser : IdentityUser
{
    public ICollection<Organization> OwnedOrganizations { get; set; } = new List<Organization>();
    public ICollection<OrganizationMember> OrganizationsMembers { get; set; } = new List<OrganizationMember>();
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    public ICollection<TaskEntity> CreatedTasks { get; set; } = new List<TaskEntity>();
    public ICollection<TaskEntity> AssignedTasks { get; set; } = new List<TaskEntity>();
}
