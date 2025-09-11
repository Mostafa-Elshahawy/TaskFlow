namespace TaskFlow.Domain.Entites;

public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
