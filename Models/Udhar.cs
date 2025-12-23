using SQLite;
using System;

namespace vyaparsathi.Models;

public class Udhar
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public bool IsPaid { get; set; } = false;
}
