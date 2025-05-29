namespace DermatologyApi.Models
{
    public class IdempotencyRecord
    {
        public string Key { get; set; }
        public string Status { get; set; }
        public string? Result { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
