using DermatologyApi.Services;
using Microsoft.AspNetCore.Mvc;
using static DermatologyApi.Services.ITransferService;

namespace DermatologyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;
        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpPost()]
        public async Task<ActionResult> TransferConsultations(TransferRequest[] transfers)
        {
            await _transferService.TransferConsultationsAsync(transfers);
            return NoContent();
        }
    }
}
