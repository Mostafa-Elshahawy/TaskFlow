using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Users.Dtos;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Select(u => new UserDto { Id = u.Id, Email = u.Email!, UserName = u.UserName })
            .ToListAsync(cancellationToken);
    }
}
