using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnacksPOS.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace SnacksPOS.Web.Pages.SignIn;

public class SignInModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SignInModel> _logger;

    public SignInModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<SignInModel> logger) 
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _logger.LogInformation("Attempting login for user: {Username}", Input.Username);
        
        // Check if user exists
        var user = await _userManager.FindByNameAsync(Input.Username);
        if (user == null)
        {
            _logger.LogWarning("User not found: {Username}", Input.Username);
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return Page();
        }

        _logger.LogInformation("User found: {Username}, attempting password verification", Input.Username);

        var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, isPersistent: true, lockoutOnFailure: false);
        
        _logger.LogInformation("Login result for {Username}: Succeeded={Succeeded}, IsLockedOut={IsLockedOut}, RequiresTwoFactor={RequiresTwoFactor}", 
            Input.Username, result.Succeeded, result.IsLockedOut, result.RequiresTwoFactor);

        if (result.Succeeded) 
        {
            _logger.LogInformation("Login successful for user: {Username}", Input.Username);
            return RedirectToPage("/Snacks/Index");
        }

        ModelState.AddModelError(string.Empty, "Invalid username or password");
        return Page();
    }
}
