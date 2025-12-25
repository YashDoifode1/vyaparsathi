using SQLite;

namespace vyaparsathi.Models;

public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    // 🔹 Prices
    public decimal LandingPrice { get; set; }
    public decimal SellingPrice { get; set; }

    // 🔹 Stock
    public int StockQuantity { get; set; }

    // 🔹 Relations
    public int CategoryId { get; set; }

    // 🔹 Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
