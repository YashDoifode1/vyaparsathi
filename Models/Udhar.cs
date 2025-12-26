using SQLite;

namespace vyaparsathi.Models;

public class Udhar
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public decimal Amount { get; set; }

    public string Notes { get; set; }

    /// <summary>
    /// Date when udhar was created
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Automatically set to Date + 7 days
    /// </summary>
    public DateTime DueDate { get; set; }

    public bool IsPaid { get; set; }

    [Ignore]
    public string CustomerName { get; set; }
}
