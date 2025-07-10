using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SnacksPOS.Application.Ledger;

namespace SnacksPOS.Web.Pages.Account;

[Authorize]
public class AccountModel : PageModel
{
    private readonly IMediator _mediator;
    public decimal Balance { get; set; }
    public bool Overdue { get; set; }
    public AccountModel(IMediator mediator) => _mediator = mediator;

    public async Task OnGetAsync()
    {
        (Balance, Overdue) = await _mediator.Send(new GetBalanceQuery(User.Identity!.Name!));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _mediator.Send(new PayBalanceCommand(User.Identity!.Name!));
        return RedirectToPage();
    }
}
