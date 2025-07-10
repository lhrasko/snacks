using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SnacksPOS.Web.Pages.Cart;

[Authorize]
public class CartModel : PageModel
{
    public void OnGet(){}
}
