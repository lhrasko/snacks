using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SnackPOS.Data;
using SnackPOS.Models;

namespace SnackPOS.Pages.Admin.Products;

[Authorize(Roles = "Admin")]
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
        Products = await _context.Products.ToListAsync();
    }
}
