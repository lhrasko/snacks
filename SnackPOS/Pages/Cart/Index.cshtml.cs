using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SnackPOS.Data;
using SnackPOS.Models;

namespace SnackPOS.Pages.Cart;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public IList<OrderItem> Items { get; set; } = new List<OrderItem>();

    public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        await LoadCartAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadCartAsync();
        if(!Items.Any()) return RedirectToPage();
        var order = new Order { UserId = _userManager.GetUserId(User)!, Items = Items.ToList() };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove("cart");
        return RedirectToPage("/Orders/Details", new { id = order.Id });
    }

    private async Task LoadCartAsync()
    {
        var cart = HttpContext.Session.GetString("cart") ?? string.Empty;
        var ids = cart.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        Items = await _context.Products.Where(p => ids.Contains(p.Id))
            .Select(p => new OrderItem { ProductId = p.Id, Product = p, Quantity = ids.Count(id => id == p.Id) })
            .ToListAsync();
    }
}
