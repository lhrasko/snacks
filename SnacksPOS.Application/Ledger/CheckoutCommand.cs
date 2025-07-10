using MediatR;
using SnacksPOS.Domain;
using SnacksPOS.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace SnacksPOS.Application.Ledger;

public record CheckoutCommand(string UserId, List<CartItem> Items) : IRequest<int>;

public class CheckoutHandler : IRequestHandler<CheckoutCommand, int>
{
    private readonly AppDbContext _db;
    public CheckoutHandler(AppDbContext db) => _db = db;

    public async Task<int> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var total = request.Items.Sum(i => i.SnapshotPrice * i.Quantity);
        var entry = new LedgerEntry
        {
            UserId = request.UserId,
            Items = request.Items,
            Total = total,
            Timestamp = DateTime.UtcNow,
            Paid = false
        };
        _db.LedgerEntries.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
        return entry.Id;
    }
}
