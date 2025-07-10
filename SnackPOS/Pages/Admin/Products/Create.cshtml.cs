using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnackPOS.Data;
using SnackPOS.Models;

namespace SnackPOS.Pages.Admin.Products;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    [BindProperty]
    public Product Product { get; set; } = new();

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if(!ModelState.IsValid) return Page();
        _context.Products.Add(Product);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
