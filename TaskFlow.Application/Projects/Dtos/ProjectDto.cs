namespace TaskFlow.Application.Projects.Dtos;

public class ProjectDto
{
    public int Id { set; get; }
    public string Name { set; get; } = default!;
    public string Description { set; get; } = default!;
    public string CreatedByUserId { set; get; } = default!;
    public string CreatedByName { set; get; } = default!;
    public DateTime CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public bool IsDeleted { set; get; } = false;
    public int OrganizationId { set; get; }
}