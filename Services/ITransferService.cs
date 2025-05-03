namespace DermatologyApi.Services
{
    public interface ITransferService
    {
        Task TransferConsultationsAsync(TransferRequest[] transfers);
        public class TransferRequest
        {
            public int ConsultationId { get; set; }
            public DateTime NewDateTime { get; set; }
        }
    }
}
