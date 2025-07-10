using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnacksPOS.Domain;

namespace SnacksPOS.Web.Pages.SignIn;

public class SignInModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    public SignInModel(SignInManager<ApplicationUser> signInManager) => _signInManager = signInManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded) return RedirectToPage("/Snacks/Index");
        ModelState.AddModelError(string.Empty, "Invalid login");
        return Page();
    }
}
