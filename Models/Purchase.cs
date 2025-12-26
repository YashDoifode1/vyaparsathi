using SQLite;

namespace vyaparsathi.Models;

public class Purchase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int VendorId { get; set; }
    public string VendorName { get; set; }

    public DateTime Date { get; set; }

    // Extra charges on the whole purchase
    public decimal Transport { get; set; }
    public decimal Tax { get; set; }
    public decimal OtherCharges { get; set; }

    // Total including items + charges
    public decimal TotalAmount { get; set; }
}
