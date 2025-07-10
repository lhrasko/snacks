using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using SnackPOS.Data;
using SnackPOS.Models;

namespace SnackPOS.Pages.Products;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public IList<Product> Products { get; set; } = new List<Product>();

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Products = await _context.Products.Where(p => p.Available).ToListAsync();
    }

    public async Task<IActionResult> OnPostAddAsync(int id)
    {
        var cart = HttpContext.Session.GetString("cart") ?? string.Empty;
        var ids = cart.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        ids.Add(id.ToString());
        HttpContext.Session.SetString("cart", string.Join(',', ids));
        return RedirectToPage();
    }
}
