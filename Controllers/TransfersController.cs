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

        [HttpPost("transfers")]
        public async Task<ActionResult> TransferConsultations(TransferRequest[] transfers)
        {
            try
            {
                await _transferService.TransferConsultationsAsync(transfers);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
