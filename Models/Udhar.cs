using SQLite;

namespace vyaparsathi.Models;

public class Udhar
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string CustomerName { get; set; }

    public decimal Amount { get; set; }

    public string Notes { get; set; }

    public DateTime Date { get; set; }
}
