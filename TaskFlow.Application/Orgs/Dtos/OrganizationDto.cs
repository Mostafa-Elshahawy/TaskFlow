namespace TaskFlow.Application.Orgs.Dtos;

public class OrganizationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string OwnerId { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
