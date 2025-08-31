namespace TaskFlow.Domain.Entites;

public class OrganizationInvitation
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = default!;
    public string InvitedEmail { get; set; } = default!;
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public OrganizationRole Role { get; set; } = OrganizationRole.Member;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Accepted { get; set; } = false;
}