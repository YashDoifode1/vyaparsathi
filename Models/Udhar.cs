using SQLite;

namespace vyaparsathi.Models;

public class Udhar
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CustomerId { get; set; } // Link to Customer

    public decimal Amount { get; set; }

    public string Notes { get; set; }

    public DateTime Date { get; set; }

    [Ignore]
    public string CustomerName { get; set; } // For display purposes
}
