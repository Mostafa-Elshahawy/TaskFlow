namespace TaskFlow.Application.Orgs.Invitations;

public sealed class InvitationOptions
{
    public const string SectionName = "Invitation";

    public string ClientBaseUrl { get; set; } = "http://localhost:4200";
    public int LifetimeHours { get; set; } = 72;
}
