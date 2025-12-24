using SQLite;

namespace vyaparsathi.Models;

public class BillItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int BillId { get; set; } // Link to Bill

    public string ItemName { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Total => Quantity * Price;
}
