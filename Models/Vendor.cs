using SQLite;

namespace vyaparsathi.Models
{
    public class Vendor
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }  // Added
        public string Phone { get; set; }    // Added
        public string Email { get; set; }    // Optional

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
