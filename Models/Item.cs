using SQLite;

namespace vyaparsathi.Models;

public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal LandingPrice { get; set; }
    public decimal SellingPrice { get; set; }

    public int Stock { get; set; }
    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
