using DermatologyApi.DTOs;
using DermatologyApi.Services;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DermatologyApi.Mappers;
using System.ComponentModel.DataAnnotations;
using DermatologyApi.Exceptions;

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
            var consultation = await _consultationService.GetConsultationEntityByIdAsync(id);
            var etag = Convert.ToBase64String(consultation.RowVersion);

            var ifNoneMatch = Request.Headers["If-None-Match"].ToString();
            if (etag == ifNoneMatch)
                return StatusCode(304);

            Response.Headers["ETag"] = etag;
            return Ok(ConsultationMapper.MapToDto(consultation));
        }

        [HttpPost]
        public async Task<ActionResult<ConsultationDto>> CreateConsultation(ConsultationCreateDto consultationDto)
        {
            var consultation = await _consultationService.CreateConsultationAsync(consultationDto);
            return CreatedAtAction(nameof(GetConsultationById), new { id = consultation.Id }, consultation);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ConsultationDto>> UpdateConsultation(int id, ConsultationUpdateDto consultationDto)
        {
            var etagBase64 = Request.Headers["If-Match"].ToString();
            if (string.IsNullOrEmpty(etagBase64))
                throw new ValidationException("ETag header is required.");

            var consultation = await _consultationService.GetConsultationEntityByIdAsync(id);
            var etag = Convert.ToBase64String(consultation.RowVersion);

            if (etagBase64 != etag)
            {
                throw new PreconditionFailedException("Precondition failed: The resource has been modified by another user.");
            }

            var updatedConsultation = await _consultationService.UpdateConsultationAsync(id, consultationDto, Convert.FromBase64String(etag));

            consultation = await _consultationService.GetConsultationEntityByIdAsync(id);
            etag = Convert.ToBase64String(consultation.RowVersion);

            Response.Headers["ETag"] = etag;

            return Ok(updatedConsultation);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteConsultation(int id)
        {
            await _consultationService.DeleteConsultationAsync(id);
            return NoContent();
        }
    }
}
