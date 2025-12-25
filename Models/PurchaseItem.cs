using SQLite;

namespace vyaparsathi.Models;

public class PurchaseItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int PurchaseId { get; set; } // FK to Purchase
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}
