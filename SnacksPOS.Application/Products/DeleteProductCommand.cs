using MediatR;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Products;

public record DeleteProductCommand(int Id) : IRequest<bool>;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly AppDbContext _db;
    public DeleteProductHandler(AppDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FindAsync(request.Id);
        if (product == null) return false;

        // Soft delete - just mark as inactive
        product.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
