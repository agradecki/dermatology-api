using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyApi.Services;
using DermatologyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using DermatologyApi.Exceptions;

namespace DermatologyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DermatologistsController : ControllerBase
    {
        private readonly IDermatologistService _dermatologistService;

        public DermatologistsController(IDermatologistService dermatologistService)
        {
            _dermatologistService = dermatologistService;
        }

        [HttpGet]
        public async Task<ActionResult<DermatologistDto>> GetAllDermatologists()
        {
            var dermatologists = await _dermatologistService.GetAllDermatologistsAsync();
            if (dermatologists == null)
                return NotFound();

            return Ok(dermatologists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DermatologistDto>> GetDermatologistsById(int id)
        {
            var dermatologist = await _dermatologistService.GetDermatologistEntityByIdAsync(id);
            if (dermatologist == null)
                return NotFound();

            var etag = dermatologist.Xmin.ToString();
            var ifNoneMatch = Request.Headers["If-None-Match"].ToString();
            if (etag == ifNoneMatch)
                return StatusCode(304);

            Response.Headers["ETag"] = etag;
            return Ok(DermatologistMapper.MapToDto(dermatologist));
        }

        [HttpPost]
        public async Task<ActionResult<DermatologistDto>> CreateDermatologist(DermatologistCreateDto dermatologistDto)
        {
            var dermatologist = await _dermatologistService.CreateDermatologistAsync(dermatologistDto);
            return CreatedAtAction(nameof(GetDermatologistsById), new { id = dermatologist.Id }, dermatologist);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DermatologistDto>> UpdateDermatologist(int id, DermatologistUpdateDto dermatologistDto)
        {
            var etagBase64 = Request.Headers["If-Match"].ToString();
            if (string.IsNullOrEmpty(etagBase64))
            {
                throw new ValidationException("ETag header is required.");
            }

            var dermatologist = await _dermatologistService.GetDermatologistEntityByIdAsync(id);
            var etag = dermatologist.Xmin.ToString();

            if (etagBase64 != etag)
            {
                throw new PreconditionFailedException("Precondition failed: The resource has been modified by another user.");
            }

            if (!uint.TryParse(etag, out uint expectedXmin))
            {
                return BadRequest("Invalid ETag format");
            }

            var updatedDermatologist = await _dermatologistService.UpdateDermatologistAsync(id, dermatologistDto, expectedXmin);

            dermatologist = await _dermatologistService.GetDermatologistEntityByIdAsync(id);
            etag = dermatologist.Xmin.ToString();

            Response.Headers["ETag"] = etag;

            return Ok(updatedDermatologist);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<DermatologistDto>> PatchDermatologist(int id, DermatologistPatchDto dermatologistDto)
        {
            var dermatologist = await _dermatologistService.PatchDermatologistAsync(id, dermatologistDto);
            return Ok(dermatologist);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDermatologist(int id)
        {
            await _dermatologistService.DeleteDermatologistAsync(id);
            return NoContent();
        }

        [HttpGet("{did}/diagnoses")]
        public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetDermatologistDiagnoses(int did)
        {
            var diagnoses = await _dermatologistService.GetDermatologistDiagnosesAsync(did);
            return Ok(diagnoses);
        }

        [HttpGet("{did}/consultations")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetDermatologistConsultations(int did)
        {
            var consultations = await _dermatologistService.GetDermatologistConsultationsAsync(did);
            return Ok(consultations);
        }
    }
}
