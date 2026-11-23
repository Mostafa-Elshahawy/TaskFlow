namespace TaskFlow.Domain.Entites;

public class OrganizationInvitation
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = default!;
    public string InvitedEmail { get; set; } = default!;
    public string TokenHash { get; set; } = default!;
    public OrganizationRole Role { get; set; } = OrganizationRole.Member;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? AcceptedAtUtc { get; set; }
    public string? AcceptedByUserId { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
