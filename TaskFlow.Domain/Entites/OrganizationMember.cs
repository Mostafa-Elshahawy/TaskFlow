namespace TaskFlow.Domain.Entites;

public class OrganizationMember
{
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public OrganizationRole Role { get; set; } = OrganizationRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public enum OrganizationRole
{
    Member,
    Manager,
    Owner
}