using SQLite;

namespace vyaparsathi.Models;

public class Bill
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string CustomerName { get; set; }

    public DateTime Date { get; set; }

    [Ignore] // Not stored in DB, calculated on the fly
    public decimal TotalAmount { get; set; }
}
