using SQLite;

namespace vyaparsathi.Models;

public class Udhar
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public decimal Amount { get; set; }

    public string Notes { get; set; }

    public DateTime Date { get; set; }

    public bool IsPaid { get; set; }  // ✅ NEW

    [Ignore]
    public string CustomerName { get; set; }
}
