using SQLite;

namespace vyaparsathi.Models;

public class PurchaseItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int PurchaseId { get; set; }
    public int ItemId { get; set; }

    public decimal Quantity { get; set; }
    public decimal Price { get; set; }        // Price per unit

    // Ignore transport at item level
    [Ignore]
    public string Name { get; set; }

    [Ignore]
    public decimal Total => Price * Quantity; // Only item total
}
