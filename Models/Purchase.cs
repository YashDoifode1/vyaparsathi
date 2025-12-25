using SQLite;

namespace vyaparsathi.Models;

public class Purchase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int VendorId { get; set; }
    public string VendorName { get; set; }

    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
}
