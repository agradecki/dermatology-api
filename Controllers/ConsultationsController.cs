using DermatologyApi.DTOs;
using DermatologyApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationsController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ConsultationDto>>> GetConsultations([FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            var consultations = await _consultationService.GetAllConsultationsAsync(page, size);
            return Ok(consultations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultationDto>> GetConsultationById(int id)
        {
            try
            {
                var consultation = await _consultationService.GetConsultationByIdAsync(id);
                return Ok(consultation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ConsultationDto>> CreateConsultation(ConsultationCreateDto consultationDto)
        {
            try
            {
                var consultation = await _consultationService.CreateConsultationAsync(consultationDto);
                return CreatedAtAction(nameof(GetConsultationById), new { id = consultation.Id }, consultation);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ConsultationDto>> UpdateConsultation(int id, ConsultationUpdateDto consultationDto)
        {
            try
            {
                var etagBase64 = Request.Headers["If-Match"].ToString();
                if (string.IsNullOrEmpty(etagBase64))
                    return BadRequest("ETag header is required");

                byte[] etag = Convert.FromBase64String(etagBase64);

                var consultation = await _consultationService.UpdateConsultationAsync(id, consultationDto, etag);
                var updatedConsultation = await _consultationService.GetConsultationEntityByIdAsync(id);
                var newEtag = Convert.ToBase64String(updatedConsultation.RowVersion);

                Response.Headers.Add("ETag", newEtag);
                return Ok(consultation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(412, "Precondition failed: The resource has been modified by another user");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteConsultation(int id)
        {
            try
            {
                await _consultationService.DeleteConsultationAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("transfers")]
        public async Task<ActionResult> TransferConsultations(TransferRequest[] transfers)
        {
            try
            {
                await _consultationService.TransferConsultationsAsync(transfers);
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
