using MediatR;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Products;

public record UpdateProductStockCommand(int Id, int Stock) : IRequest<bool>;

public class UpdateProductStockHandler : IRequestHandler<UpdateProductStockCommand, bool>
{
    private readonly AppDbContext _db;
    public UpdateProductStockHandler(AppDbContext db) => _db = db;

    public async Task<bool> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FindAsync(request.Id);
        if (product == null) return false;
        product.Stock = request.Stock;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
