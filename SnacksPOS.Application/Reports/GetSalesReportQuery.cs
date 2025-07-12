using MediatR;
using Microsoft.EntityFrameworkCore;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Reports;

public record GetSalesReportQuery(DateTime? StartDate, DateTime? EndDate, string? UserId) : IRequest<SalesReportResult>;

public record SalesReportResult(
    decimal TotalSales,
    int TotalTransactions,
    decimal AverageTransactionValue,
    List<ProductSalesItem> TopProducts,
    List<DailySalesItem> DailySales,
    List<UserSalesItem> UserSales
);

public record ProductSalesItem(int ProductId, string ProductName, int Quantity, decimal Revenue);
public record DailySalesItem(DateTime Date, decimal Sales, int Transactions);
public record UserSalesItem(string UserId, string UserEmail, decimal Sales, int Transactions);

public class GetSalesReportHandler : IRequestHandler<GetSalesReportQuery, SalesReportResult>
{
    private readonly AppDbContext _db;
    public GetSalesReportHandler(AppDbContext db) => _db = db;

    public async Task<SalesReportResult> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.Today.AddDays(-30);
        var endDate = request.EndDate ?? DateTime.Today.AddDays(1);

        var query = _db.LedgerEntries
            .Where(l => l.Timestamp >= startDate && l.Timestamp < endDate && l.Total > 0);

        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(l => l.UserId == request.UserId);
        }

        var entries = await query.ToListAsync(cancellationToken);

        var totalSales = entries.Sum(e => e.Total);
        var totalTransactions = entries.Count;
        var averageTransaction = totalTransactions > 0 ? totalSales / totalTransactions : 0;

        var productSales = entries
            .SelectMany(e => e.Items)
            .GroupBy(i => i.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.Quantity * i.SnapshotPrice)
            })
            .ToList();

        // Get product names for the top selling products
        var productIds = productSales.OrderByDescending(p => p.Revenue).Take(10).Select(p => p.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var topProducts = productSales
            .Join(products, ps => ps.ProductId, p => p.Id, (ps, p) => new ProductSalesItem(ps.ProductId, p.Name, ps.Quantity, ps.Revenue))
            .OrderByDescending(p => p.Revenue)
            .Take(10)
            .ToList();

        // Daily sales
        var dailySales = entries
            .GroupBy(e => e.Timestamp.Date)
            .Select(g => new DailySalesItem(g.Key, g.Sum(e => e.Total), g.Count()))
            .OrderBy(d => d.Date)
            .ToList();

        // User sales
        var userSales = entries
            .GroupBy(e => e.UserId)
            .Select(g => new UserSalesItem(g.Key, g.Key, g.Sum(e => e.Total), g.Count()))
            .OrderByDescending(u => u.Sales)
            .ToList();

        return new SalesReportResult(
            totalSales,
            totalTransactions,
            averageTransaction,
            topProducts,
            dailySales,
            userSales
        );
    }
}
