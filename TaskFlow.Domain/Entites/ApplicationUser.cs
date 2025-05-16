using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Domain.Entites;

public class ApplicationUser : IdentityUser
{
    public ICollection<Organization> OwnedOrganizations { get; set; } = default!;
    public ICollection<Organization> MemberOrganizations { get; set; } = default!;
    public ICollection<Project> AssignedProjects { get; set; } = default!;
    public ICollection<Project> ManagedProjects { get; set; } = default!;
    public ICollection<Task> AssignedTasks { get; set; } = default!;
    public ICollection<Task> CreatedTasks { get; set; } = default!;
}
