using DermatologyApi.DTOs;
using DermatologyApi.Services;
using DermatologyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var dermatologist = await _dermatologistService.GetDermatologistByIdAsync(id);
                return Ok(dermatologist);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DermatologistDto>> CreateDermatologist(DermatologistCreateDto dermatologistDto)
        {
            var dermatologist = await _dermatologistService.CreateDermatologistAsync(dermatologistDto);
            return CreatedAtAction(nameof(Dermatologist), new { id = dermatologist.Id }, dermatologist);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DermatologistDto>> UpdateDermatologist(int id, DermatologistUpdateDto dermatologistDto)
        {
            try
            {
                var etagBase64 = Request.Headers["If-Match"].ToString();
                if (string.IsNullOrEmpty(etagBase64))
                    return BadRequest("ETag header is required");

                byte[] etag = Convert.FromBase64String(etagBase64);

                var dermatologist = await _dermatologistService.UpdateDermatologistAsync(id, dermatologistDto, etag);
                var updatedDermatologist = await _dermatologistService.GetDermatologistEntityByIdAsync(id);
                var newEtag = Convert.ToBase64String(updatedDermatologist.RowVersion);

                Response.Headers.Add("ETag", newEtag);

                return Ok(dermatologist);
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

        [HttpPatch("{id}")]
        public async Task<ActionResult<DermatologistDto>> PatchDermatologist(int id, DermatologistPatchDto dermatologistDto)
        {
            try
            {
                var dermatologist = await _dermatologistService.PatchDermatologistAsync(id, dermatologistDto);
                return Ok(dermatologist);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDermatologist(int id)
        {
            try
            {
                await _dermatologistService.DeleteDermatologistAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{did}/diagnoses")]
        public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetDermatologistDiagnoses(int did)
        {
            try
            {
                var diagnoses = await _dermatologistService.GetDermatologistDiagnosesAsync(did);
                if (diagnoses == null || !diagnoses.Any())
                    return NotFound($"No diagnoses found for dermatologist with ID {did}.");

                return Ok(diagnoses);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Dermatologist with ID {did} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred {ex.Message}");
            }
        }

        [HttpGet("{did}/consultations")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetDermatologistConsultations(int did)
        {
            try
            {
                var consultations = await _dermatologistService.GetDermatologistConsultationsAsync(did);
                if (consultations == null || !consultations.Any())
                    return NotFound($"No consultations found for dermatologist with ID {did}.");

                return Ok(consultations);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Dermatologist with ID {did} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred {ex.Message}");
            }
        }
    }
}
