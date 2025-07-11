using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnacksPOS.Application.Products;
using SnacksPOS.Domain;

namespace SnacksPOS.Web.Pages.Snacks;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class SnacksModel : PageModel
{
    private readonly IMediator _mediator;
    public List<Product> Products { get; set; } = new();
    public SnacksModel(IMediator mediator) => _mediator = mediator;
    public async Task OnGetAsync() => Products = await _mediator.Send(new GetProductsQuery());
}
