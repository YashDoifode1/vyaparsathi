using SQLite;

namespace vyaparsathi.Models;

public class Category
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }    // <-- Add this
    public DateTime UpdatedAt { get; set; }    // <-- Add this
}
