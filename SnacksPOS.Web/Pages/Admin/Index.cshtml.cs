using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SnacksPOS.Web.Pages.Admin;

[Authorize(Roles="Admin")]
public class AdminModel : PageModel
{
    public void OnGet(){}
}
