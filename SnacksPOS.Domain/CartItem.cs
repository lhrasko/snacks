namespace SnacksPOS.Domain;

public class CartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal SnapshotPrice { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CartItem item &&
               ProductId == item.ProductId &&
               Quantity == item.Quantity &&
               SnapshotPrice == item.SnapshotPrice;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ProductId, Quantity, SnapshotPrice);
    }
}
