using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Users;

public record GetUsersQuery(int PageNumber = 1, int PageSize = 20) : IRequest<UsersResult>;

public record UsersResult(List<UserItem> Users, int TotalCount, int PageNumber, int PageSize);

public record UserItem(string Id, string Email, bool IsEmailConfirmed, DateTime? LastLogin, List<string> Roles);

public class GetUsersHandler : IRequestHandler<GetUsersQuery, UsersResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public GetUsersHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UsersResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _userManager.Users.CountAsync(cancellationToken);

        var userItems = new List<UserItem>();
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userItems.Add(new UserItem(
                user.Id,
                user.Email ?? "",
                user.EmailConfirmed,
                null, // LastLogin would need to be tracked separately
                roles.ToList()
            ));
        }

        return new UsersResult(userItems, totalCount, request.PageNumber, request.PageSize);
    }
}
