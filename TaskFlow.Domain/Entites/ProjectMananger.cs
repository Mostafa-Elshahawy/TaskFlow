namespace TaskFlow.Domain.Entites;

public class ProjectMananger
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project ManagedProject { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
}
