namespace DermatologyAPI.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public int ConsultationId { get; set; }
        public DateTime NewDateTime { get; set; }
    }
}
