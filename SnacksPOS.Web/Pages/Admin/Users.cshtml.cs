using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SnacksPOS.Web.Pages.Admin;

[Authorize(Roles="Admin")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class UsersModel : PageModel
{
    public void OnGet(){}
}
