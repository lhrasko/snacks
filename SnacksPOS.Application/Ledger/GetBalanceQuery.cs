using MediatR;
using SnacksPOS.Domain;
using SnacksPOS.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace SnacksPOS.Application.Ledger;

public record GetBalanceQuery(string UserId) : IRequest<(decimal Balance, bool Overdue)>;

public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, (decimal, bool)>
{
    private readonly AppDbContext _db;
    public GetBalanceHandler(AppDbContext db) => _db = db;

    public async Task<(decimal, bool)> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var entries = await _db.LedgerEntries.Where(l => l.UserId == request.UserId).ToListAsync(cancellationToken);
        var balance = entries.Where(l => !l.Paid).Sum(l => l.Total);
        var overdue = entries.Any(l => !l.Paid && l.Timestamp < new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1).AddDays(-1));
        return (balance, overdue);
    }
}
