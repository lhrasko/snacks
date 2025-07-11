using MediatR;
using SnacksPOS.Domain;
using Microsoft.EntityFrameworkCore;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Products;

public record UpdateProductCommand(int Id, string Name, string? Description, decimal Price, string? ImageUrl, bool IsActive) : IRequest<Product?>;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Product?>
{
    private readonly AppDbContext _db;
    public UpdateProductHandler(AppDbContext db) => _db = db;

    public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FindAsync(request.Id);
        if (product == null) return null;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        return product;
    }
}
