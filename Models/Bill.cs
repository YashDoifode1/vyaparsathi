using SQLite;
using System;

namespace vyaparsathi.Models;

public class Bill
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string CustomerMobile { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public string ItemsJson { get; set; }
}
