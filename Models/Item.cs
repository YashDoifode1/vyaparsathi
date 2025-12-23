using SQLite;

namespace vyaparsathi.Models;

public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int StockQuantity { get; set; }
}
