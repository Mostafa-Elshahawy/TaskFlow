using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Domain.Entites;

public class Project
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(500)]
    public string Description { get; set; } = default!;
    public string CreatedByUserId { get; set; } = default!;
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = default!;
    public ApplicationUser CreatedBy { get; set; } = default!;
    public ICollection<ApplicationUser> Managers { get; set; } = default!;
    public ICollection<ApplicationUser> Members { get; set; } = default!;
    public ICollection<TaskEntity> Tasks { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool isDeleted { get; set; }
}
