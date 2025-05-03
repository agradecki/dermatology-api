using DermatologyApi.DTOs;
using DermatologyApi.Services;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DermatologyApi.Mappers;

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
                var consultation = await _consultationService.GetConsultationEntityByIdAsync(id);
                var etag = Convert.ToBase64String(consultation.RowVersion);

                var ifNoneMatch = Request.Headers["If-None-Match"].ToString();
                if (etag == ifNoneMatch)
                    return StatusCode(304);

                Response.Headers["ETag"] = etag;
                return Ok(ConsultationMapper.MapToDto(consultation));
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

                if (consultation == null)
                    return NotFound("Patient or dermatologist not found.");

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

                var consultation = await _consultationService.GetConsultationEntityByIdAsync(id);
                var etag = Convert.ToBase64String(consultation.RowVersion);

                if (etagBase64 != etag)
                {
                    return StatusCode(412, "Precondition failed: The resource has been modified by another user.");
                }

                var updatedConsultation = await _consultationService.UpdateConsultationAsync(id, consultationDto, Convert.FromBase64String(etag));

                consultation = await _consultationService.GetConsultationEntityByIdAsync(id);
                etag = Convert.ToBase64String(consultation.RowVersion);

                Response.Headers["ETag"] = etag;

                return Ok(updatedConsultation);
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
    }
}
