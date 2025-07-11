using MediatR;
using Microsoft.AspNetCore.Identity;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Users;

public record UpdateUserCommand(string UserId, string Email, List<string> Roles, bool IsActive) : IRequest<UpdateUserResult>;

public record UpdateUserResult(bool Success, List<string> Errors);

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UpdateUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return new UpdateUserResult(false, new List<string> { "User not found" });
        }

        // Update email
        if (user.Email != request.Email)
        {
            user.Email = request.Email;
            user.UserName = request.Email;
            var updateResult = await _userManager.UpdateAsync(user);
            
            if (!updateResult.Succeeded)
            {
                return new UpdateUserResult(false, updateResult.Errors.Select(e => e.Description).ToList());
            }
        }

        // Update roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = request.Roles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(request.Roles).ToList();

        if (rolesToRemove.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        }

        if (rolesToAdd.Any())
        {
            await _userManager.AddToRolesAsync(user, rolesToAdd);
        }

        return new UpdateUserResult(true, new List<string>());
    }
}
