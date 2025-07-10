using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SnackPOS.Data;

namespace SnackPOS.Pages.Admin;

[Authorize(Roles = "Admin")]
public class StatsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }

    public StatsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        TotalOrders = await _context.Orders.CountAsync();
        TotalRevenue = await _context.Orders
            .SelectMany(o => o.Items)
            .SumAsync(i => i.Quantity * i.Product!.Price);
    }
}
