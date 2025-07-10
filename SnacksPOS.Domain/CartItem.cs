namespace SnacksPOS.Domain;

public class CartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal SnapshotPrice { get; set; }
}
