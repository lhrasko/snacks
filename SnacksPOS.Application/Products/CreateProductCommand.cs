using MediatR;
using SnacksPOS.Domain;
using SnacksPOS.Infrastructure;

namespace SnacksPOS.Application.Products;

public record CreateProductCommand(string Name, string? Description, decimal Price, string? ImageUrl, int Stock) : IRequest<Product>;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly AppDbContext _db;
    public CreateProductHandler(AppDbContext db) => _db = db;

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            Stock = request.Stock,
            IsActive = true
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);
        return product;
    }
}
