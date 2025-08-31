using MediatR;

namespace TaskFlow.Application.Orgs.Commnads;

public class CreateOrgCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string AdminEmail { get; set; } = default!;
    public string AdminPassword { get; set; } = default!;
}
