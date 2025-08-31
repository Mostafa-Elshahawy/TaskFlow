using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Domain.Entites;

public class Organization 
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(500)]
    public string Description { get; set; } = default!;
    public string OwnerId { get; set; } = default!;
    public ApplicationUser Owner { get; set; } = default!;
    public ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
    public ICollection<Project> Projects { get; set; } = default!;
    public ICollection<OrganizationInvitation> Invitations { get; set; } = new List<OrganizationInvitation>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool isDeleted { get; set; }
}
