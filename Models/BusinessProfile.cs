using SQLite;

namespace vyaparsathi.Models;

public class BusinessProfile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string BusinessName { get; set; }
    public string OwnerName { get; set; }
    public string Phone { get; set; }
    public string WhatsApp { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
