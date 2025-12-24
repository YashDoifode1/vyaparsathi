using SQLite;

namespace vyaparsathi.Models;

public class Customer
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Gender { get; set; }

    public string Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
