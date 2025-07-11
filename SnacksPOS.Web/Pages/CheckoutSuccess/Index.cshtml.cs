using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnacksPOS.Application.Ledger;

namespace SnacksPOS.Web.Pages.CheckoutSuccess;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    public decimal Balance { get; set; }
    public bool Overdue { get; set; }
    public DateTime DueDate { get; set; }
    
    public IndexModel(IMediator mediator) => _mediator = mediator;

    public async Task OnGet()
    {
        (Balance, Overdue) = await _mediator.Send(new GetBalanceQuery(User.Identity!.Name!));
        
        // Calculate due date (end of current month)
        var now = DateTime.Now;
        DueDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
    }
}
