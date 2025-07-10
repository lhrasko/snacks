using MediatR;
using SnacksPOS.Domain;
using Microsoft.EntityFrameworkCore;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Products;

public record GetProductsQuery : IRequest<List<Product>>;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<Product>>
{
    private readonly AppDbContext _db;
    public GetProductsHandler(AppDbContext db) => _db = db;

    public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Products.Where(p => p.IsActive).AsNoTracking().ToListAsync(cancellationToken);
    }
}
