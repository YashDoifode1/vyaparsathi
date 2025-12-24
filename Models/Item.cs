using SQLite;

namespace vyaparsathi.Models;

public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    // Category Relation
    public int CategoryId { get; set; }

    // Pricing
    public decimal LandingPrice { get; set; }
    public decimal SellingPrice { get; set; }

    // Stock
    public int StockQuantity { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
