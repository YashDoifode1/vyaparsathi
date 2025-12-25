using SQLite;

namespace vyaparsathi.Models
{
    public class SmsSettings
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ApiKey { get; set; }
        public string SenderId { get; set; }
        public string Url { get; set; }
    }
}
