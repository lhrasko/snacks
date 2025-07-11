using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SignOutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    public SignOutModel(SignInManager<ApplicationUser> signInManager) => _signInManager = signInManager;

    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/SignIn/Index");
    }
}
