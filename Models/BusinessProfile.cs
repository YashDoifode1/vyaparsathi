using SQLite;

namespace vyaparsathi.Models;

public class BusinessProfile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Basic Business Info
    public string BusinessName { get; set; }
    public string OwnerName { get; set; }
    public string Phone { get; set; }
    public string WhatsApp { get; set; }

    // SMS API Configuration
    public string SmsApiKey { get; set; }
    public string SmsSenderId { get; set; }
    public string SmsApiUrl { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
