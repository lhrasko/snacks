using MediatR;
using SnacksPOS.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace SnacksPOS.Application.Ledger;

public record PayBalanceCommand(string UserId) : IRequest<int>;

public class PayBalanceHandler : IRequestHandler<PayBalanceCommand, int>
{
    private readonly AppDbContext _db;
    public PayBalanceHandler(AppDbContext db) => _db = db;

    public async Task<int> Handle(PayBalanceCommand request, CancellationToken cancellationToken)
    {
        var entries = await _db.LedgerEntries.Where(l => l.UserId == request.UserId && !l.Paid).ToListAsync(cancellationToken);
        foreach (var e in entries)
        {
            e.Paid = true;
        }
        return await _db.SaveChangesAsync(cancellationToken);
    }
}
