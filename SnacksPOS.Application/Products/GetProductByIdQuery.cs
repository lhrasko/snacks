using MediatR;
using SnacksPOS.Domain;
using Microsoft.EntityFrameworkCore;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Products;

public record GetProductByIdQuery(int Id) : IRequest<Product?>;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly AppDbContext _db;
    public GetProductByIdHandler(AppDbContext db) => _db = db;

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
    }
}
