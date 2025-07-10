namespace SnacksPOS.Domain;

public class LedgerEntry
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal Total { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Paid { get; set; }
}
