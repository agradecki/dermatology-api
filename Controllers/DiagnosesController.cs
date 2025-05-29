using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyApi.Services;
using DermatologyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using DermatologyApi.Exceptions;
using System.ComponentModel;

namespace DermatologyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosesController : ControllerBase
    {
        private readonly IDiagnosisService _diagnosisService;

        public DiagnosesController(IDiagnosisService diagnosisService)
        {
            _diagnosisService = diagnosisService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetDiagnoses()
        {
            var diagnoses = await _diagnosisService.GetAllDiagnosesAsync();
            return Ok(diagnoses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiagnosisDto>> GetDiagnosisById(int id)
        {
            var diagnosis = await _diagnosisService.GetDiagnosisEntityByIdAsync(id);
            if (diagnosis == null)
                return NotFound();

            var etag = Convert.ToBase64String(diagnosis.RowVersion);
            var ifNoneMatch = Request.Headers["If-None-Match"].ToString();
            if (etag == ifNoneMatch)
                return StatusCode(304);

            Response.Headers["ETag"] = etag;
            return Ok(DiagnosisMapper.MapToDto(diagnosis));
        }

        [HttpPost]
        public async Task<ActionResult<DiagnosisDto>> CreateDiagnosis(DiagnosisCreateDto diagnosisDto, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
        {
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                return BadRequest("Idempotency-Key header is required");
            }
            
            var diagnosis = await _diagnosisService.CreateDiagnosisWithIdempotencyAsync(diagnosisDto, idempotencyKey);
            return CreatedAtAction(nameof(GetDiagnosisById), new { id = diagnosis.Id }, diagnosis);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DiagnosisDto>> UpdateDiagnosis(int id, DiagnosisUpdateDto diagnosisDto)
        {
            var etagBase64 = Request.Headers["If-Match"].ToString();
            if (string.IsNullOrEmpty(etagBase64))
            {
                throw new ValidationException("ETag header is required.");
            }

            var diagnosis = await _diagnosisService.GetDiagnosisEntityByIdAsync(id);
            var etag = Convert.ToBase64String(diagnosis.RowVersion);

            if (etagBase64 != etag)
            {
                throw new PreconditionFailedException("Precondition failed: The resource has been modified by anther user.");
            }

            var updatedDiagnosis = await _diagnosisService.UpdateDiagnosisAsync(id, diagnosisDto, Convert.FromBase64String(etag));

            diagnosis = await _diagnosisService.GetDiagnosisEntityByIdAsync(id);
            etag = Convert.ToBase64String(diagnosis.RowVersion);

            Response.Headers["ETag"] = etag;
            return Ok(updatedDiagnosis);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDiagnosis(int id)
        {
            await _diagnosisService.DeleteDiagnosisAsync(id);
            return NoContent();
        }
    }
}
