using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnacksPOS.Domain;

namespace SnacksPOS.Web.Pages;

public class SignOutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    public SignOutModel(SignInManager<ApplicationUser> signInManager) => _signInManager = signInManager;

    public async Task OnPost()
    {
        await _signInManager.SignOutAsync();
    }
}
