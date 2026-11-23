using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Authorization;
using TaskFlow.Application.Users.Dtos;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(
    UserManager<ApplicationUser> userManager,
    IWorkspaceAccessService accessService) : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var currentUser = accessService.GetCurrentUser();

        return await userManager.Users
            .Where(user => user.Id == currentUser.Id ||
                           user.OrganizationsMembers.Any(userMembership =>
                               userMembership.Organization.Members.Any(currentMembership =>
                                   currentMembership.UserId == currentUser.Id)))
            .Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName
            })
            .ToListAsync(cancellationToken);
    }
}
