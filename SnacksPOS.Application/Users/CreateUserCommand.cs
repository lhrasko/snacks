using MediatR;
using Microsoft.AspNetCore.Identity;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Users;

public record CreateUserCommand(string Email, string Password, List<string> Roles) : IRequest<CreateUserResult>;

public record CreateUserResult(bool Success, string? UserId, List<string> Errors);

public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public CreateUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return new CreateUserResult(false, null, result.Errors.Select(e => e.Description).ToList());
        }

        // Add roles if specified
        if (request.Roles.Any())
        {
            await _userManager.AddToRolesAsync(user, request.Roles);
        }

        return new CreateUserResult(true, user.Id, new List<string>());
    }
}
