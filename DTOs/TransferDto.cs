using System.ComponentModel.DataAnnotations;

namespace DermatologyApi.DTOs
{
    public class TransferRequestDto
    {
        [Required]
        public required List<TransferItem> Transfers { get; set; }
    }
    public class TransferItem
    {
        [Required]
        public int ConsultationId { get; set; }

        [Required]
        public DateTime NewDateTime { get; set; }

    }
}
